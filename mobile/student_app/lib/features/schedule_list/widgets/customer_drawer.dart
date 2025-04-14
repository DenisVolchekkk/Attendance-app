import 'package:auto_route/auto_route.dart';
import 'package:flutter/material.dart';
import 'package:student_app/router/router.dart';

class DayUtils {
  static const Map<int, String> daysOfWeek = {
    1: 'Понедельник',
    2: 'Вторник',
    3: 'Среда',
    4: 'Четверг',
    5: 'Пятница',
    6: 'Суббота',
    0: 'Воскресенье',
  };

  static String getDayName(int dayNumber) {
    return daysOfWeek[dayNumber] ?? 'Unknown';
  }

  static int? getDayNumber(String dayName) {
    for (final entry in daysOfWeek.entries) {
      if (entry.value == dayName) {
        return entry.key;
      }
    }
    return null;
  }

  static List<int> getDayNumbers() {
    return daysOfWeek.keys.toList();
  }

  static List<String> getDayNames() {
    return daysOfWeek.values.toList();
  }
}

class CustomDrawer extends StatelessWidget {
  final ThemeData theme;
  final VoidCallback onHomeTap;
  final VoidCallback onSettingsTap;

  const CustomDrawer({
    super.key,
    required this.theme,
    required this.onHomeTap,
    required this.onSettingsTap,
  });

  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: <Widget>[
          DrawerHeader(
            decoration: BoxDecoration(color: theme.primaryColor),
            child: const Text('Меню', style: TextStyle(color: Colors.white, fontSize: 24)),
          ),
          ListTile(
            leading: const Icon(Icons.home),
            title: const Text('Расписание'),
            onTap: () {
              AutoRouter.of(context).push(ScheduleRoute());
            },
          ), 
          ListTile(
            leading: const Icon(Icons.home),
            title: const Text('Посещения'),
            onTap: () {
              AutoRouter.of(context).push(AttendanceRoute());
            },
          ),
          ListTile(
            leading: const Icon(Icons.home),
            title: const Text('Home'),
            onTap: () {
              Navigator.pop(context);
              onHomeTap();
            },
          ),
          ListTile(
            leading: const Icon(Icons.settings),
            title: const Text('Settings'),
            onTap: () {
              Navigator.pop(context);
              onSettingsTap();
            },
          ),
        ],
      ),
    );
  }
}