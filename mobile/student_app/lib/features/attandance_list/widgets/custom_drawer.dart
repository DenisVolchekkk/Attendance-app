import 'package:flutter/material.dart';
import 'package:student_app/router/router.dart';
import 'package:auto_route/auto_route.dart';

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
  final int? selectedDayNumber;
  final String? selectedGroupName;
  final String? selectedDiscipline;
  final TimeOfDay? selectedTime;
  final DateTime? selectedDate;
  final ValueChanged<int> onDaySelected;
  final ValueChanged<String> onGroupSelected;
  final ValueChanged<String> onDisciplineSelected;
  final ValueChanged<TimeOfDay> onTimeSelected;
  final ValueChanged<DateTime> onDateSelected;
  final VoidCallback onHomeTap;
  final VoidCallback onSettingsTap;
  final List<String> availableGroups;
  final List<String> availableDisciplines;

  const CustomDrawer({
    super.key,
    required this.theme,
    this.selectedDayNumber,
    this.selectedGroupName,
    this.selectedDiscipline,
    this.selectedTime,
    this.selectedDate,
    required this.onDaySelected,
    required this.onGroupSelected,
    required this.onDisciplineSelected,
    required this.onTimeSelected,
    required this.onDateSelected,
    required this.onHomeTap,
    required this.onSettingsTap,
    required this.availableGroups,
    required this.availableDisciplines,
  });

Future<void> _selectTime(BuildContext context) async {
  final TimeOfDay? picked = await showTimePicker(
    context: context,
    initialTime: selectedTime ?? TimeOfDay.now(),
    builder: (BuildContext context, Widget? child) {
      return MediaQuery(
        data: MediaQuery.of(context).copyWith(alwaysUse24HourFormat: true),
        child: child!,
      );
    },
  );
  if (picked != null) {
    onTimeSelected(picked);
  }
}

  Future<void> _selectDate(BuildContext context) async {
    final DateTime? picked = await showDatePicker(
      context: context,
      initialDate: selectedDate ?? DateTime.now(),
      firstDate: DateTime(2000),
      lastDate: DateTime(2100),
    );
    if (picked != null) {
      onDateSelected(picked);
    }
  }

  @override
  Widget build(BuildContext context) {
    return Drawer(
      child: ListView(
        padding: EdgeInsets.zero,
        children: <Widget>[
          DrawerHeader(
            decoration: BoxDecoration(color: theme.primaryColor),
            child: const Text('Меню', style: TextStyle(color: Colors.white, fontSize: 24))),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
            child: DropdownButtonFormField<int>(
              decoration: const InputDecoration(
                labelText: 'Выбор дня',
                hintText: 'Выберите день',
                border: OutlineInputBorder(),
              ),
              value: selectedDayNumber,
              items: DayUtils.daysOfWeek.entries.map((entry) {
                return DropdownMenuItem<int>(
                  value: entry.key,
                  child: Text(entry.value),
                );
              }).toList(),
              onChanged: (value) {
                if (value != null) {
                  onDaySelected(value);
                }
              },
              isExpanded: true,
            ),
          ),
          if (availableGroups.isNotEmpty) ...[
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
              child: DropdownButtonFormField<String>(
                decoration: const InputDecoration(
                  labelText: 'Выбор группы',
                  hintText: 'Выберите группу',
                  border: OutlineInputBorder(),
                ),
                value: selectedGroupName,
                items: availableGroups.map((group) {
                  return DropdownMenuItem<String>(
                    value: group,
                    child: Text(group),
                  );
                }).toList(),
                onChanged: (value) {
                  if (value != null) {
                    onGroupSelected(value);
                  }
                },
                isExpanded: true,
              ),
            ),
          ],
          if (availableDisciplines.isNotEmpty) ...[
            Padding(
              padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
              child: DropdownButtonFormField<String>(
                decoration: const InputDecoration(
                  labelText: 'Выбор дисциплины',
                  hintText: 'Выберите дисциплину',
                  border: OutlineInputBorder(),
                ),
                value: selectedDiscipline,
                items: availableDisciplines.map((discipline) {
                  return DropdownMenuItem<String>(
                    value: discipline,
                    child: Text(discipline,
                                  style: const TextStyle(color: Colors.black),),
                  );
                }).toList(),
                onChanged: (value) {
                  if (value != null) {
                    onDisciplineSelected(value);
                  }
                },
                isExpanded: true,
              ),
            ),
          ],
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
            child: InkWell(
              onTap: () => _selectTime(context),
              child: InputDecorator(
                decoration: const InputDecoration(
                  labelText: 'Время занятия',
                  border: OutlineInputBorder(),
                ),
                child: Text(selectedTime?.format(context) ?? 'Выберите время',
                                  style: const TextStyle(color: Colors.black),),
              ),
            ),
          ),
          Padding(
            padding: const EdgeInsets.symmetric(horizontal: 16.0, vertical: 8.0),
            child: InkWell(
              onTap: () => _selectDate(context),
              child: InputDecorator(
                decoration: const InputDecoration(
                  labelText: 'Дата занятия',
                  border: OutlineInputBorder(),
                ),
                child: Text(selectedDate != null 
                  ? '${selectedDate!.day}.${selectedDate!.month}.${selectedDate!.year}' 
                  : 'Выберите дату',
                                  style: const TextStyle(color: Colors.black),),
              ),
            ),
          ),
          const Divider(),
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
            title: const Text('Гайд'),
            onTap: () {
              AutoRouter.of(context).push(GuideRoute());
              onHomeTap();
            },
          ),

        ],
      ),
    );
  }
}