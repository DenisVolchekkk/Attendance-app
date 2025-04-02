import 'package:flutter/material.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/router/router.dart';
import 'package:student_app/theme/theme.dart';
import 'package:talker_flutter/talker_flutter.dart';

class StudentsApp extends StatefulWidget {
  const StudentsApp({super.key});

  @override
  State<StudentsApp> createState() => _StudentsApp();
}
class _StudentsApp extends State<StudentsApp> {
  final _appRouter = AppRouter();
  @override
  Widget build(BuildContext context) {
    return MaterialApp.router(
      title: 'Flutter Demo',
      theme: dartTheme,
      routerConfig: _appRouter.config(
        navigatorObservers: () => [
          TalkerRouteObserver(GetIt.I<Talker>()),
        ],
      ),
    );
  }
}
