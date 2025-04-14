import 'package:flutter/material.dart';
import 'package:student_app/repositories/models/models.dart';
import 'widgets.dart';

class ScheduleTable extends StatelessWidget {
  final List<Schedule> schedules;
  final int? selectedDayNumber;
  final String? selectedGroupName;
  final String? selectedDiscipline;
  final TimeOfDay? selectedTime;
  final Function(BuildContext, Schedule) onItemTap;

  const ScheduleTable({
    super.key,
    required this.schedules,
    this.selectedDayNumber,
    this.selectedGroupName,
    this.selectedDiscipline,
    this.selectedTime,
    required this.onItemTap,
  });

  @override
  Widget build(BuildContext context) {
    // Фильтрация по выбранным параметрам
    var filteredSchedules = schedules;
    
    if (selectedDayNumber != null) {
      filteredSchedules = filteredSchedules.where((s) => s.dayOfWeek == selectedDayNumber).toList();
    }
    
    if (selectedGroupName != null) {
      filteredSchedules = filteredSchedules.where((s) => s.group.name == selectedGroupName).toList();
    }
    
    if (selectedDiscipline != null) {
      filteredSchedules = filteredSchedules.where((s) => s.discipline.name == selectedDiscipline).toList();
    }
    
    if (selectedTime != null) {
      filteredSchedules = filteredSchedules.where((s) => 
        s.startTime.hour == selectedTime!.hour && 
        s.startTime.minute == selectedTime!.minute
      ).toList();
    }

    // Группировка по дням недели
    final Map<int, List<Schedule>> scheduleByDay = {};
    for (var schedule in filteredSchedules) {
      scheduleByDay.putIfAbsent(schedule.dayOfWeek, () => []);
      scheduleByDay[schedule.dayOfWeek]!.add(schedule);
    }

    // Сортируем дни недели по порядку (пн=1, вт=2, ..., вс=7)
    final sortedDays = scheduleByDay.keys.toList()
      ..sort((a, b) => a.compareTo(b));

    return ListView(
      children: [
        for (var day in sortedDays)
          ScheduleDayCard(
            day: day,
            daySchedules: scheduleByDay[day]!,
            onItemTap: onItemTap,
          ),
      ],
    );
  }
}