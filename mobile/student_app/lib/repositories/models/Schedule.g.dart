// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'Schedule.dart';

// **************************************************************************
// TypeAdapterGenerator
// **************************************************************************

class ScheduleAdapter extends TypeAdapter<Schedule> {
  @override
  final int typeId = 5;

  @override
  Schedule read(BinaryReader reader) {
    final numOfFields = reader.readByte();
    final fields = <int, dynamic>{
      for (int i = 0; i < numOfFields; i++) reader.readByte(): reader.read(),
    };
    return Schedule(
      id: fields[0] as int,
      startTime: fields[1] as TimeOfDay,
      endTime: fields[2] as TimeOfDay,
      dayOfWeek: fields[3] as int,
      semestr: fields[4] as int,
      studyYear: fields[5] as int,
      auditory: fields[6] as String,
      groupId: fields[7] as int,
      group: fields[8] as Group,
      teacherId: fields[9] as int,
      teacher: fields[10] as Teacher,
      disciplineId: fields[11] as int,
      discipline: fields[12] as Discipline,
    );
  }

  @override
  void write(BinaryWriter writer, Schedule obj) {
    writer
      ..writeByte(13)
      ..writeByte(0)
      ..write(obj.id)
      ..writeByte(1)
      ..write(obj.startTime)
      ..writeByte(2)
      ..write(obj.endTime)
      ..writeByte(3)
      ..write(obj.dayOfWeek)
      ..writeByte(4)
      ..write(obj.semestr)
      ..writeByte(5)
      ..write(obj.studyYear)
      ..writeByte(6)
      ..write(obj.auditory)
      ..writeByte(7)
      ..write(obj.groupId)
      ..writeByte(8)
      ..write(obj.group)
      ..writeByte(9)
      ..write(obj.teacherId)
      ..writeByte(10)
      ..write(obj.teacher)
      ..writeByte(11)
      ..write(obj.disciplineId)
      ..writeByte(12)
      ..write(obj.discipline);
  }

  @override
  int get hashCode => typeId.hashCode;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is ScheduleAdapter &&
          runtimeType == other.runtimeType &&
          typeId == other.typeId;
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Schedule _$ScheduleFromJson(Map<String, dynamic> json) => Schedule(
      id: (json['id'] as num).toInt(),
      startTime:
          const TimeOfDayConverter().fromJson(json['startTime'] as String),
      endTime: const TimeOfDayConverter().fromJson(json['endTime'] as String),
      dayOfWeek: (json['dayOfWeek'] as num).toInt(),
      semestr: (json['semestr'] as num).toInt(),
      studyYear: (json['studyYear'] as num).toInt(),
      auditory: json['auditory'] as String,
      groupId: (json['groupId'] as num).toInt(),
      group: Group.fromJson(json['group'] as Map<String, dynamic>),
      teacherId: (json['teacherId'] as num).toInt(),
      teacher: Teacher.fromJson(json['teacher'] as Map<String, dynamic>),
      disciplineId: (json['disciplineId'] as num).toInt(),
      discipline:
          Discipline.fromJson(json['discipline'] as Map<String, dynamic>),
    );

Map<String, dynamic> _$ScheduleToJson(Schedule instance) => <String, dynamic>{
      'id': instance.id,
      'startTime': const TimeOfDayConverter().toJson(instance.startTime),
      'endTime': const TimeOfDayConverter().toJson(instance.endTime),
      'dayOfWeek': instance.dayOfWeek,
      'semestr': instance.semestr,
      'studyYear': instance.studyYear,
      'auditory': instance.auditory,
      'groupId': instance.groupId,
      'group': instance.group,
      'teacherId': instance.teacherId,
      'teacher': instance.teacher,
      'disciplineId': instance.disciplineId,
      'discipline': instance.discipline,
    };
