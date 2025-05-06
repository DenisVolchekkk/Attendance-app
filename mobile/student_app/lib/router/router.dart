import 'package:auto_route/auto_route.dart';
import 'package:student_app/features/attandance_list/views/views.dart';
import 'package:student_app/features/schedule_list/views/views.dart';
import 'package:student_app/features/registration/views/views.dart';
import 'package:student_app/features/others/views/guide_screen.dart';

import 'package:flutter/material.dart';
part 'router.gr.dart';

@AutoRouterConfig()
class AppRouter extends RootStackRouter {
  @override
  List<AutoRoute> get routes => [
        AutoRoute(page: AttendanceRoute.page, ),
        AutoRoute(page: LoginRoute.page, path: '/'),
        AutoRoute(page: RegistrationRoute.page,),
        AutoRoute(page: ScheduleRoute.page,),
        AutoRoute(page: GuideRoute.page,),
      ];
}
