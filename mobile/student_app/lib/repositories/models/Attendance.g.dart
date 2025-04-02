// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'Attendance.dart';

// **************************************************************************
// TypeAdapterGenerator
// **************************************************************************

class AttendanceAdapter extends TypeAdapter<Attendance> {
  @override
  final int typeId = 6;

  @override
  Attendance read(BinaryReader reader) {
    final numOfFields = reader.readByte();
    final fields = <int, dynamic>{
      for (int i = 0; i < numOfFields; i++) reader.readByte(): reader.read(),
    };
    return Attendance(
      id: fields[0] as int,
      studentId: fields[1] as int,
      student: fields[2] as Student,
      attendanceDate: fields[3] as DateTime,
      isPresent: fields[4] as bool,
      scheduleId: fields[5] as int,
      schedule: fields[6] as Schedule,
    );
  }

  @override
  void write(BinaryWriter writer, Attendance obj) {
    writer
      ..writeByte(7)
      ..writeByte(0)
      ..write(obj.id)
      ..writeByte(1)
      ..write(obj.studentId)
      ..writeByte(2)
      ..write(obj.student)
      ..writeByte(3)
      ..write(obj.attendanceDate)
      ..writeByte(4)
      ..write(obj.isPresent)
      ..writeByte(5)
      ..write(obj.scheduleId)
      ..writeByte(6)
      ..write(obj.schedule);
  }

  @override
  int get hashCode => typeId.hashCode;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is AttendanceAdapter &&
          runtimeType == other.runtimeType &&
          typeId == other.typeId;
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Attendance _$AttendanceFromJson(Map<String, dynamic> json) => Attendance(
      id: (json['id'] as num).toInt(),
      studentId: (json['studentId'] as num).toInt(),
      student: Student.fromJson(json['student'] as Map<String, dynamic>),
      attendanceDate: DateTime.parse(json['attendanceDate'] as String),
      isPresent: json['isPresent'] as bool,
      scheduleId: (json['scheduleId'] as num).toInt(),
      schedule: Schedule.fromJson(json['schedule'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$AttendanceToJson(Attendance instance) =>
    <String, dynamic>{
      'id': instance.id,
      'studentId': instance.studentId,
      'student': instance.student,
      'attendanceDate': instance.attendanceDate.toIso8601String(),
      'isPresent': instance.isPresent,
      'scheduleId': instance.scheduleId,
      'schedule': instance.schedule,
    };
