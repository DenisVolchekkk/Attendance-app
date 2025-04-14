import 'package:flutter/material.dart';

import '../models/models.dart';

abstract class AbstractAttandanceRepository {
  Future<List<Attendance>> getAttandanceList({    int? day, 
    String? groupName,
    String? discipline,
    TimeOfDay? attendanceTime,
    DateTime? attendanceDate,});
  Future<void> isPresentStudent(Attendance attendance);
  Future<List<String>> getTeacherGroups();
  Future<List<String>> getTeacherDisciplines();
}