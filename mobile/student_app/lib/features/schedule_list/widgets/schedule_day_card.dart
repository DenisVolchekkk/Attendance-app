import 'package:flutter/material.dart';
import 'package:student_app/repositories/models/models.dart';

import 'widgets.dart';

class ScheduleDayCard extends StatelessWidget {
  final int day;
  final List<Schedule> daySchedules;
  final Function(BuildContext, Schedule) onItemTap;

  const ScheduleDayCard({
    super.key,
    required this.day,
    required this.daySchedules,
    required this.onItemTap,
  });

  @override
  Widget build(BuildContext context) {
    // Сначала сортируем по номеру пары, затем по времени начала
    daySchedules.sort((a, b) {
      final pairNumberA = _getPairNumber(a.startTime);
      final pairNumberB = _getPairNumber(b.startTime);
      if (pairNumberA != pairNumberB) {
        return pairNumberA.compareTo(pairNumberB);
      }
      return a.startTime.hour.compareTo(b.startTime.hour);
    });

    return Card(
      margin: const EdgeInsets.all(8.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Padding(
            padding: const EdgeInsets.all(8.0),
            child: Text(
              DayUtils.getDayName(day),
              style: const TextStyle(
                fontSize: 18,
                fontWeight: FontWeight.bold,
                color: Colors.black,
              ),
            ),
          ),
          Table(
            border: TableBorder.all(),
            columnWidths: const {
              0: FlexColumnWidth(1.5),
              1: FlexColumnWidth(0.5),
              2: FlexColumnWidth(2),
            },
            children: [
              const TableRow(
                decoration: BoxDecoration(color: Colors.grey),
                children: [
                  TableCell(
                      child: Center(
                          child: Text('Время',
                              style: TextStyle(color: Colors.black)))),
                  TableCell(
                      child: Center(
                          child: Text('№',
                              style: TextStyle(color: Colors.black)))),
                  TableCell(
                      child: Center(
                          child: Text('Занятие',
                              style: TextStyle(color: Colors.black)))),
                ],
              ),
              for (var schedule in daySchedules)
                TableRow(
                  children: [
                    TableCell(
                      child: InkWell(
                        onTap: () => onItemTap(context, schedule),
                        child: Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Text(
                            '${_formatTime(schedule.startTime)}-${_formatTime(schedule.endTime)}',
                            textAlign: TextAlign.center,
                            style: const TextStyle(color: Colors.black),
                          ),
                        ),
                      ),
                    ),
                    TableCell(
                      child: InkWell(
                        onTap: () => onItemTap(context, schedule),
                        child: Center(
                          child: Text(
                            '${_getPairNumber(schedule.startTime)}',
                            style: const TextStyle(color: Colors.black),
                          ),
                        ),
                      ),
                    ),
                    TableCell(
                      child: InkWell(
                        onTap: () => onItemTap(context, schedule),
                        child: Padding(
                          padding: const EdgeInsets.all(8.0),
                          child: Column(
                            crossAxisAlignment: CrossAxisAlignment.start,
                            children: [
                              Text(
                                schedule.discipline.name,
                                style: const TextStyle(
                                  fontWeight: FontWeight.bold,
                                  color: Colors.black,
                                ),
                              ),
                              if (schedule.teacher.name.isNotEmpty)
                                Text(
                                  '${schedule.teacher.name}',
                                  style: const TextStyle(color: Colors.black),
                                ),
                              if (schedule.group.name.isNotEmpty)
                                Text(
                                  'Группа: ${schedule.group.name}',
                                  style: const TextStyle(color: Colors.black),
                                ),
                              Text(
                                'Аудитория: ${schedule.auditory}',
                                style: const TextStyle(color: Colors.black),
                              ),
                            ],
                          ),
                        ),
                      ),
                    ),
                  ],
                ),
            ],
          ),
        ],
      ),
    );
  }

  String _formatTime(TimeOfDay time) {
    return '${time.hour.toString().padLeft(2, '0')}.${time.minute.toString().padLeft(2, '0')}';
  }

  int _getPairNumber(TimeOfDay startTime) {
    if (startTime.hour == 8 && startTime.minute == 20) return 1;
    if (startTime.hour == 9 && startTime.minute == 5) return 2;
    if (startTime.hour == 10 && startTime.minute == 0) return 3;
    if (startTime.hour == 10 && startTime.minute == 45) return 4;
    if (startTime.hour == 11 && startTime.minute == 35) return 5;
    if (startTime.hour == 12 && startTime.minute == 20) return 6;
    if (startTime.hour == 13 && startTime.minute == 30) return 7;
    if (startTime.hour == 14 && startTime.minute == 15) return 8;
    if (startTime.hour == 15 && startTime.minute == 5) return 9;
    if (startTime.hour == 15 && startTime.minute == 50) return 10;
    if (startTime.hour == 16 && startTime.minute == 45) return 11;
    if (startTime.hour == 17 && startTime.minute == 30) return 12;
    if (startTime.hour == 18 && startTime.minute == 20) return 13;
    if (startTime.hour == 19 && startTime.minute == 5) return 14;
    if (startTime.hour == 20 && startTime.minute == 5) return 15;
    if (startTime.hour == 20 && startTime.minute == 50) return 16;
    return 0;
  }
}
