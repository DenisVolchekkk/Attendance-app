import 'package:hive_flutter/adapters.dart';
import 'package:equatable/equatable.dart';
import 'package:json_annotation/json_annotation.dart';

part 'Group.g.dart';

@HiveType(typeId: 2)
@JsonSerializable()
class Group extends Equatable{
   const Group({
    required this.id,
    required this.name,
    required this.chiefId,
    required this.facilityId,
  });
  @HiveField(0)
  final int id;
  @HiveField(1)
  final String name;
  @HiveField(2)
  final int chiefId;
  @HiveField(3)
  final int facilityId;

  factory Group.fromJson(Map<String, dynamic> json) => _$GroupFromJson(json);

  Map<String, dynamic> toJson() =>  _$GroupToJson(this); 

  @override
  List<Object> get props => [id, name, chiefId, facilityId];


}