import 'package:dio/dio.dart';
import 'package:get_it/get_it.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:student_app/repositories/attandance/abstract_attandance_repository.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:talker_flutter/talker_flutter.dart';
import 'dart:convert';
import 'package:flutter/material.dart';

class AttandanceRepository implements AbstractAttandanceRepository {
  AttandanceRepository({
    required this.dio,
    required this.attandanceBox,
  });

  final Dio dio;
  final LazyBox<Attendance> attandanceBox;

 @override
  Future<List<Attendance>> getAttandanceList({
    int? day, 
    String? groupName,
    String? discipline,
    TimeOfDay? attendanceTime,
    DateTime? attendanceDate,
  }) async {
    var attendanceList = <Attendance>[];
    try {
      attendanceList = await _fetchAttandanceListFromApi(
        day: day,
        groupName: groupName,
        discipline: discipline,
        attendanceTime: attendanceTime,
        attendanceDate: attendanceDate,
      );
      final cryptoCoinsMap = {for (var e in attendanceList) e.id: e};
      await attandanceBox.putAll(cryptoCoinsMap);
    } catch (e, st) {
      GetIt.instance<Talker>().handle(e, st);
      attendanceList = await getAttendanceListToLazyBox();
    }
    return attendanceList;
  }

  @override
  Future<void> isPresentStudent(Attendance attendance) async {
    final isUpdated = await _putAttendance(attendance);
    if (!isUpdated) {
      throw Exception('Failed to update attendance');
    }
  }
 Future<List<Attendance>> _fetchAttandanceListFromApi({int? day, String? groupName, 
 String? discipline, TimeOfDay? attendanceTime, DateTime? attendanceDate}) async {
    final token = await getToken();
    if (token == null) {
      throw Exception('Token is null');
    }


    final queryParameters = <String, dynamic>{};

    final decodedToken = _decodeJwt(token);
    final roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    final isTeacher = roles is List ? roles.contains('Teacher') : roles == 'Teacher';
        // Добавляем фильтр по учителю, если это учитель
    if (isTeacher) {
      final teacherName = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      queryParameters['Schedule.Teacher.Name'] = teacherName;
    }

    if (groupName != null) {
      queryParameters['Schedule.Group.Name'] = groupName;
    }
    if (day != null) {
      queryParameters['Schedule.DayOfWeek'] = day;
    }

    if (discipline != null) {
      queryParameters['Schedule.Discipline.Name'] = discipline;
    }
    if (attendanceDate != null) {
      queryParameters['AttendanceDate'] = attendanceDate;
    } 
    if (attendanceTime != null) {
      final timeString = '${attendanceTime.hour.toString().padLeft(2, '0')}:'
                        '${attendanceTime.minute.toString().padLeft(2, '0')}';
      queryParameters['Schedule.StartTime'] = timeString;
    } 
    // Добавляем фильтр по группе, если указана


    final response = await dio.get(
      'http://ggtuapi.runasp.net/api/Attendance/Filter',
      queryParameters: queryParameters,
      options: Options(
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      ),
    );
    
    final List<dynamic> data = response.data;
    return data.map((json) => Attendance.fromJson(json)).toList();
  }



  Future<bool> _putAttendance(Attendance attendance) async {
      final jsonData = {
        ...attendance.toJson(), // Копируем все поля из toJson()
        'isPresent': !attendance.isPresent, // Инвертируем isPresent
      };
      final token = await getToken();
      final response = await dio.put(
        'http://ggtuapi.runasp.net/api/Attendance/Put', // Убедитесь, что URL правильный
        data: jsonData, // Преобразуем объект Attendance в JSON
        options: Options(
          headers: {
            'Content-Type': 'application/json', // Указываем тип контента
            'Authorization': 'Bearer $token', // Добавляем токен в заголовок Authorization
          },
        ),
      );
        if (response.statusCode == 200) {
      return true; // Успешное обновление
    } else {
      throw Exception('Failed to update attendance: ${response.statusCode}');
    }
  } 
  Future<List<Attendance>> getAttendanceListToLazyBox() async {
    final attendanceBox = await Hive.openBox<Attendance>('attendanceBox');

    final attendanceList = await Future.wait(
      attendanceBox.keys.map((key) async {
        final attendance = attendanceBox
            .get(key); // This could return Attendance? (nullable)
        return attendance; // We are expecting Attendance, but it might be null
      }),
    );

    return attendanceList.cast<Attendance>();
  }
   @override
  Future<List<String>> getTeacherGroups() async {
    final token = await getToken();
    if (token == null) {
      throw Exception('Token is null');
    }

    final queryParameters = <String, dynamic>{};
    final decodedToken = _decodeJwt(token);
    final roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    final isTeacher = roles is List ? roles.contains('Teacher') : roles == 'Teacher';
        // Добавляем фильтр по учителю, если это учитель
    if (isTeacher) {
      final teacherName = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      queryParameters['Schedule.Teacher.Name'] = teacherName;
    }


    // Получаем все посещения для текущего учителя
    final response = await dio.get(
      'http://ggtuapi.runasp.net/api/Attendance/Filter',
      queryParameters: queryParameters,
      options: Options(
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      ),
    );
    
    final List<dynamic> data = response.data;
    final attendances = data.map((json) => Attendance.fromJson(json)).toList();
    
    // Извлекаем уникальные имена групп
    final groups = attendances
        .map((a) => a.schedule.group.name)
        .toSet()
        .toList();
    
    return groups;
  }
  @override
  Future<List<String>> getTeacherDisciplines() async {
    final token = await getToken();
    if (token == null) {
      throw Exception('Token is null');
    }
    
    final queryParameters = <String, dynamic>{};

    final decodedToken = _decodeJwt(token);
    final roles = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    final isTeacher = roles is List ? roles.contains('Teacher') : roles == 'Teacher';
        // Добавляем фильтр по учителю, если это учитель
    if (isTeacher) {
      final teacherName = decodedToken['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name'];
      queryParameters['Schedule.Teacher.Name'] = teacherName;
    }
    final response = await dio.get(
      'http://ggtuapi.runasp.net/api/Attendance/Filter',
      queryParameters: queryParameters,
      options: Options(
        headers: {
          'Content-Type': 'application/json',
          'Authorization': 'Bearer $token',
        },
      ),
    );
    
    final List<dynamic> data = response.data;
    final attendances = data.map((json) => Attendance.fromJson(json)).toList();
    
    final disciplines = attendances
        .map((a) => a.schedule.discipline.name)
        .toSet()
        .toList();
    
    return disciplines;
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
