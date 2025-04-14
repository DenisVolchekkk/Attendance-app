// GENERATED CODE - DO NOT MODIFY BY HAND

// **************************************************************************
// AutoRouterGenerator
// **************************************************************************

// ignore_for_file: type=lint
// coverage:ignore-file

part of 'router.dart';

/// generated route for
/// [AttendanceScreen]
class AttendanceRoute extends PageRouteInfo<AttendanceRouteArgs> {
  AttendanceRoute({
    Key? key,
    int? day,
    String? groupName,
    String? discipline,
    TimeOfDay? time,
    DateTime? date,
    List<PageRouteInfo>? children,
  }) : super(
          AttendanceRoute.name,
          args: AttendanceRouteArgs(
            key: key,
            day: day,
            groupName: groupName,
            discipline: discipline,
            time: time,
            date: date,
          ),
          initialChildren: children,
        );

  static const String name = 'AttendanceRoute';

  static PageInfo page = PageInfo(
    name,
    builder: (data) {
      final args = data.argsAs<AttendanceRouteArgs>(
          orElse: () => const AttendanceRouteArgs());
      return AttendanceScreen(
        key: args.key,
        day: args.day,
        groupName: args.groupName,
        discipline: args.discipline,
        time: args.time,
        date: args.date,
      );
    },
  );
}

class AttendanceRouteArgs {
  const AttendanceRouteArgs({
    this.key,
    this.day,
    this.groupName,
    this.discipline,
    this.time,
    this.date,
  });

  final Key? key;

  final int? day;

  final String? groupName;

  final String? discipline;

  final TimeOfDay? time;

  final DateTime? date;

  @override
  String toString() {
    return 'AttendanceRouteArgs{key: $key, day: $day, groupName: $groupName, discipline: $discipline, time: $time, date: $date}';
  }
}

/// generated route for
/// [LoginScreen]
class LoginRoute extends PageRouteInfo<void> {
  const LoginRoute({List<PageRouteInfo>? children})
      : super(
          LoginRoute.name,
          initialChildren: children,
        );

  static const String name = 'LoginRoute';

  static PageInfo page = PageInfo(
    name,
    builder: (data) {
      return const LoginScreen();
    },
  );
}

/// generated route for
/// [RegistrationScreen]
class RegistrationRoute extends PageRouteInfo<void> {
  const RegistrationRoute({List<PageRouteInfo>? children})
      : super(
          RegistrationRoute.name,
          initialChildren: children,
        );

  static const String name = 'RegistrationRoute';

  static PageInfo page = PageInfo(
    name,
    builder: (data) {
      return const RegistrationScreen();
    },
  );
}

/// generated route for
/// [ScheduleScreen]
class ScheduleRoute extends PageRouteInfo<void> {
  const ScheduleRoute({List<PageRouteInfo>? children})
      : super(
          ScheduleRoute.name,
          initialChildren: children,
        );

  static const String name = 'ScheduleRoute';

  static PageInfo page = PageInfo(
    name,
    builder: (data) {
      return const ScheduleScreen();
    },
  );
}
