// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';
import 'package:hive_flutter/adapters.dart';
import 'package:json_annotation/json_annotation.dart';

import 'package:student_app/repositories/models/Schedule.dart';
import 'package:student_app/repositories/models/Student.dart';

part 'Attendance.g.dart';

@HiveType(typeId: 6)
@JsonSerializable()
class Attendance extends Equatable {
  const Attendance({
    required this.id,
    required this.studentId,
    required this.student,
    required this.attendanceDate,
    required this.isPresent,
    required this.scheduleId,
    required this.schedule,
  });
  @HiveField(0)
  final int id;
  @HiveField(1)
  final int studentId;
  @HiveField(2)
  final Student student;
  @HiveField(3)
  final DateTime attendanceDate;
  @HiveField(4)
  final bool isPresent;
  @HiveField(5)
  final int scheduleId;
  @HiveField(6)
  final Schedule schedule;

  factory Attendance.fromJson(Map<String, dynamic> json) => _$AttendanceFromJson(json);

  Map<String, dynamic> toJson() =>  _$AttendanceToJson(this); 

  @override
  List<Object> get props => [id, studentId, student, attendanceDate, isPresent, scheduleId, schedule];

  
}
