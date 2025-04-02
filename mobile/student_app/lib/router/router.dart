import 'package:auto_route/auto_route.dart';
import 'package:student_app/features/attandance_list/views/views.dart';
import 'package:student_app/features/registration/views/views.dart';

part 'router.gr.dart';

@AutoRouterConfig()
class AppRouter extends RootStackRouter {
  @override
  List<AutoRoute> get routes => [
        AutoRoute(page: AttendanceRoute.page, ),
        AutoRoute(page: LoginRoute.page, path: '/'),
        AutoRoute(page: RegistrationRoute.page,),
      ];
}
