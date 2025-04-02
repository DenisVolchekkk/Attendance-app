// ignore_for_file: public_member_api_docs, sort_constructors_first
import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:hive_flutter/adapters.dart';
import 'package:json_annotation/json_annotation.dart';
import 'package:student_app/repositories/models/TimeOfDayCoverter.dart';
import 'package:student_app/repositories/models/models.dart';

part 'Schedule.g.dart';

@HiveType(typeId: 5)
@JsonSerializable()
@TimeOfDayConverter()
class Schedule extends Equatable  {
  const Schedule({
    required this.id,
    required this.startTime,
    required this.endTime,
    required this.dayOfWeek,
    required this.groupId,
    required this.group,
    required this.teacherId,
    required this.teacher,
    required this.disciplineId,
    required this.discipline,
  });
  @HiveField(0)
  final int id;
  @HiveField(1)
  final TimeOfDay  startTime;
  @HiveField(2)
  final  TimeOfDay  endTime;
  @HiveField(3)
  final int dayOfWeek;
  @HiveField(4)
  final int groupId;
  @HiveField(5)
  final Group group;
  @HiveField(6)
  final int teacherId; 
  @HiveField(7)
  final Teacher teacher; 
  @HiveField(8)
  final int disciplineId;
  @HiveField(9)
  final Discipline discipline;

  factory Schedule.fromJson(Map<String, dynamic> json) => _$ScheduleFromJson(json);

  Map<String, dynamic> toJson() =>  _$ScheduleToJson(this); 

  @override
  List<Object> get props => [id, startTime, endTime, dayOfWeek, groupId, teacherId, disciplineId];



}
