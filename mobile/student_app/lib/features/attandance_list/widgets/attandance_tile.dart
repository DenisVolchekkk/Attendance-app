import 'dart:async';

import 'package:flutter/material.dart';
import 'package:intl/intl.dart';
import 'package:student_app/features/attandance_list/bloc/attandance_list_bloc.dart';
import 'package:student_app/repositories/models/models.dart';
const daysOfWeek = ['Воскресенье', 'Понедельник', 'Вторник', 'Среда', 'Четверг', 'Пятница', 'Суббота'];
class AttandanceTile extends StatelessWidget {
  const AttandanceTile({
    super.key,
    required this.attendance,
    required this.bloc,
  });

  final Attendance attendance;
  final AttandanceListBloc bloc; 

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);
    
    return ListTile(
      title: Text(
        attendance.student.name,
        style: theme.textTheme.bodyMedium,
      ),
      subtitle: Text(
        '${DateFormat('yyyy-MM-dd').format(attendance.attendanceDate)} ${daysOfWeek[attendance.schedule.dayOfWeek]} ${attendance.schedule.startTime.hour.toString().padLeft(2, '0')}.${attendance.schedule.startTime.minute.toString().padLeft(2, '0')}'+
        '-${attendance.schedule.endTime.hour.toString().padLeft(2, '0')}.${attendance.schedule.endTime.minute.toString().padLeft(2, '0')} ${attendance.schedule.discipline.name}',
        style: theme.textTheme.labelSmall,
      ),
      trailing: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          Checkbox(
            value: attendance.isPresent,
            onChanged: (bool? value) {
              if (value != null) {
                final completer = Completer();
                bloc.add(UpdateAttendanceEvent(attendance, completer));
                
              }
            },
          ),
          const Icon(Icons.arrow_forward_ios),
        ],
      ),
    );
  }
}
