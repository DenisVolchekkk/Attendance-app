part of 'schedule_list_bloc.dart';

abstract class ScheduleListEvent extends Equatable {}

class LoadScheduleList extends ScheduleListEvent {
  final int? day;
  final String? groupName;
  final String? discipline;
  final TimeOfDay? startTime;
  final Completer? completer;
  
  LoadScheduleList({
    this.day,
    this.groupName,
    this.discipline,
    this.startTime,
    this.completer,
  });
  
  @override
  List<Object?> get props => [day, groupName, discipline, startTime, completer];
}