import 'dart:async';
import 'package:dio/io.dart';
import 'package:dio/dio.dart';
import 'package:firebase_core/firebase_core.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:hive_flutter/adapters.dart';
import 'package:student_app/firebase_options.dart';
import 'package:student_app/repositories/attandance/abstract_attandance_repository.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:student_app/repositories/autorize/abstract_autorize_repository.dart';
import 'package:student_app/repositories/repositories.dart';
import 'package:student_app/students_app.dart';
import 'package:talker_bloc_logger/talker_bloc_logger.dart';
import 'package:talker_dio_logger/talker_dio_logger.dart';
import 'package:talker_flutter/talker_flutter.dart';

const attandanceBoxName = 'attandance_box';
void main() async {
  
  final talker = TalkerFlutter.init();
  GetIt.I.registerSingleton(talker);
  GetIt.I<Talker>().debug('Talker started');
  GetIt.I<Talker>().error('Talker started');
  GetIt.I<Talker>().info('Talker started');

  await Hive.initFlutter();
  Hive.registerAdapter(TimeOfDayAdapter());
  Hive.registerAdapter(TeacherAdapter());
  Hive.registerAdapter(DisciplineAdapter());
  Hive.registerAdapter(GroupAdapter());
  Hive.registerAdapter(StudentAdapter());
  Hive.registerAdapter(ScheduleAdapter());
  Hive.registerAdapter(AttendanceAdapter());

  final attandanceBox = await Hive.openLazyBox<Attendance>(attandanceBoxName);

  final app = await Firebase.initializeApp(
    options: DefaultFirebaseOptions.currentPlatform,
  );
  talker.info(app.options.projectId);
  
  final dio = Dio();
    (dio.httpClientAdapter as IOHttpClientAdapter).onHttpClientCreate =
      (client) {
    client.badCertificateCallback = (cert, host, port) => true;
    return client;
  };
  dio.interceptors.add(
    TalkerDioLogger(
      talker: talker,
      settings: TalkerDioLoggerSettings(
        printResponseData: false,
      ),
    ),
    );
  dio.interceptors.add(InterceptorsWrapper(
    onRequest: (options, handler) {
      debugPrint("Request: ${options.method} ${options.uri}");
      return handler.next(options);
    },
    onResponse: (response, handler) {
      debugPrint("Response: ${response.statusCode} ${response.data}");
      return handler.next(response);
    },
    onError: (DioException e, handler) {
      debugPrint("DioError: ${e.message}");
      return handler.next(e);
    },
  ));
  Bloc.observer = TalkerBlocObserver(
    talker: talker,
    settings: TalkerBlocLoggerSettings(
      printStateFullData: false,
      printEventFullData: false,
    ),
  );


  GetIt.I.registerLazySingleton<AbstractAttandanceRepository>(
    () => AttandanceRepository(
      dio: dio,
      attandanceBox: attandanceBox,
    ),
  );
  
  GetIt.I.registerLazySingleton<AbstractAutorizeRepository>(
    () => AutorizeRepository(
      dio: dio,
    ),
  );

  FlutterError.onError =
      (details) => GetIt.I<Talker>().handle(details.exception, details.stack);

  runZonedGuarded(() => runApp(const StudentsApp()), (e, st) {
    GetIt.I<Talker>().handle(e, st);
  });
}
