import 'package:hive_flutter/adapters.dart';
import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'Discipline.g.dart';

@HiveType(typeId: 3)
@JsonSerializable()
class Discipline extends Equatable{
   const Discipline({
    required this.id,
    required this.name,
  });
  @HiveField(0)
  final int id;
  @HiveField(1)
  final String name;


  factory Discipline.fromJson(Map<String, dynamic> json) => _$DisciplineFromJson(json);

  Map<String, dynamic> toJson() =>  _$DisciplineToJson(this); 

  @override
  List<Object> get props => [id, name];


}