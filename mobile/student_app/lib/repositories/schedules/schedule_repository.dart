import 'package:dio/dio.dart';
import 'package:get_it/get_it.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:student_app/repositories/schedules/abstract_schedule_repository.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:talker_flutter/talker_flutter.dart';
import 'dart:convert';

class ScheduleRepository implements AbstractScheduleRepository {
  ScheduleRepository({
    required this.dio,
    required this.scheduleBox,
  });

  final Dio dio;
  final LazyBox<Schedule> scheduleBox;

  @override
  Future<List<Schedule>> getScheduleList()async {
    var scheduleList = <Schedule>[];
    try {
      scheduleList = await _fetchScheduleListFromApi();
      final cryptoCoinsMap = {for (var e in scheduleList) e.id: e};
      await scheduleBox.putAll(cryptoCoinsMap);
    } catch (e, st) {
      GetIt.instance<Talker>().handle(e, st);
      scheduleList = await getscheduleListToLazyBox();
    }
    return scheduleList;
  }
  
  Future<List<Schedule>> _fetchScheduleListFromApi() async {
    final token = await getToken();
    if (token == null) {
      throw Exception('Token is null');
    }
    
    final decodedToken = _decodeJwt(token);
    final roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    final isTeacher = roles is List ? roles.contains('Teacher') : roles == 'Teacher';
    final queryParameters = <String, dynamic>{};
        // Добавляем фильтр по учителю, если это учитель
    if (isTeacher ) {
      final teacherName = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      queryParameters['Schedule.Teacher.Name'] = teacherName;
    }

    final response = await dio.get(
      'http://192.168.0.105:5183/api/Schedule/Filter',
      queryParameters: queryParameters,
      options: Options(
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      ),
    );
    
    final List<dynamic> data = response.data;
    return data.map((json) => Schedule.fromJson(json)).toList();
  }

  Future<List<Schedule>> getscheduleListToLazyBox() async {
    final scheduleBox = await Hive.openBox<Schedule>('scheduleBox');

    final scheduleList = await Future.wait(
      scheduleBox.keys.map((key) async {
        final schedule = scheduleBox
            .get(key); // This could return schedule? (nullable)
        return schedule; // We are expecting schedule, but it might be null
      }),
    );

    return scheduleList.cast<Schedule>();
  }
  // Функция для декодирования JWT токена
  Map<String, dynamic> _decodeJwt(String token) {
    final parts = token.split('.');
    if (parts.length != 3) {
      throw Exception('Invalid token');
    }
    
    final payload = parts[1];
    final normalized = base64Url.normalize(payload);
    final decoded = utf8.decode(base64Url.decode(normalized));
    
    return jsonDecode(decoded);
  }
  Future<String?> getToken() async {
      final prefs = await SharedPreferences.getInstance();
      String? token = prefs.getString('auth_token');
      return token;
  }
}
