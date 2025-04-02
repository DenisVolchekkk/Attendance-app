import 'package:dio/dio.dart';
import 'package:get_it/get_it.dart';
import 'package:hive_flutter/hive_flutter.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:student_app/repositories/attandance/abstract_attandance_repository.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:talker_flutter/talker_flutter.dart';

class AttandanceRepository implements AbstractAttandanceRepository {
  AttandanceRepository({
    required this.dio,
    required this.attandanceBox,
  });

  final Dio dio;
  final LazyBox<Attendance> attandanceBox;

  @override
  Future<List<Attendance>> getAttandanceList() async {
    var attendanceList = <Attendance>[];
    try {
      attendanceList = await _fetchAttandanceListFromApi();
      final cryptoCoinsMap = {for (var e in attendanceList) e.studentId: e};
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
  Future<List<Attendance>> _fetchAttandanceListFromApi() async {
    final token = await getToken();
    final response =
        await dio.get('http://192.168.48.51:5183/api/Attendance/GetAll',
                options: Options(
          headers: {
            'Content-Type': 'application/json', // Указываем тип контента
            'Authorization': 'Bearer $token', // Добавляем токен в заголовок Authorization
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
        'http://192.168.48.51:5183/api/Attendance/Put', // Убедитесь, что URL правильный
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
  Future<String?> getToken() async {
      final prefs = await SharedPreferences.getInstance();
      String? token = prefs.getString('auth_token');
      return token;
  }
}
