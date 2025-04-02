import 'package:hive_flutter/adapters.dart';
import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'Student.g.dart';

@HiveType(typeId: 4)
@JsonSerializable()
class Student extends Equatable{
   const Student({
    required this.id,
    required this.name,
    required this.groupId,
  });
  @HiveField(0)
  final int id;
  @HiveField(1)
  final String name ;
  @HiveField(2)
  final int groupId ;

  factory Student.fromJson(Map<String, dynamic> json) => _$StudentFromJson(json);

  Map<String, dynamic> toJson() =>  _$StudentToJson(this); 

  @override
  List<Object> get props => [id, name, groupId];


}