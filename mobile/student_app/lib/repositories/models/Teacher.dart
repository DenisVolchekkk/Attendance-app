import 'package:hive_flutter/adapters.dart';
import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'Teacher.g.dart';

@HiveType(typeId: 1)
@JsonSerializable()
class Teacher extends Equatable{
   const Teacher({
    required this.id,
    required this.name,
  });
  @HiveField(0)
  final int id;
  @HiveField(1)
  final String name;


  factory Teacher.fromJson(Map<String, dynamic> json) => _$TeacherFromJson(json);

  Map<String, dynamic> toJson() =>  _$TeacherToJson(this); 

  @override
  List<Object> get props => [id, name];


}