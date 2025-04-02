// GENERATED CODE - DO NOT MODIFY BY HAND

part of 'Discipline.dart';

// **************************************************************************
// TypeAdapterGenerator
// **************************************************************************

class DisciplineAdapter extends TypeAdapter<Discipline> {
  @override
  final int typeId = 3;

  @override
  Discipline read(BinaryReader reader) {
    final numOfFields = reader.readByte();
    final fields = <int, dynamic>{
      for (int i = 0; i < numOfFields; i++) reader.readByte(): reader.read(),
    };
    return Discipline(
      id: fields[0] as int,
      name: fields[1] as String,
    );
  }

  @override
  void write(BinaryWriter writer, Discipline obj) {
    writer
      ..writeByte(2)
      ..writeByte(0)
      ..write(obj.id)
      ..writeByte(1)
      ..write(obj.name);
  }

  @override
  int get hashCode => typeId.hashCode;

  @override
  bool operator ==(Object other) =>
      identical(this, other) ||
      other is DisciplineAdapter &&
          runtimeType == other.runtimeType &&
          typeId == other.typeId;
}

// **************************************************************************
// JsonSerializableGenerator
// **************************************************************************

Discipline _$DisciplineFromJson(Map<String, dynamic> json) => Discipline(
      id: (json['id'] as num).toInt(),
      name: json['name'] as String,
    );

Map<String, dynamic> _$DisciplineToJson(Discipline instance) =>
    <String, dynamic>{
      'id': instance.id,
      'name': instance.name,
    };
