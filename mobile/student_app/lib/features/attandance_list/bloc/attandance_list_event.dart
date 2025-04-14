part of 'attandance_list_bloc.dart';

abstract class AttandanceListEvent extends Equatable {}

class LoadAttandanceList extends AttandanceListEvent {
  final int? day;
  final String? groupName;
  final String? discipline;
  final TimeOfDay? attendanceTime;
  final DateTime? attendanceDate;
  final Completer? completer;
  
  LoadAttandanceList({
    this.day,
    this.groupName,
    this.discipline,
    this.attendanceTime,
    this.attendanceDate,
    this.completer,
  });
  
  @override
  List<Object?> get props => [day, groupName, discipline, attendanceTime, attendanceDate, completer];
}

class UpdateAttendanceEvent extends AttandanceListEvent {
  final Attendance attendance;
  final Completer? completer;
  
  UpdateAttendanceEvent(
    this.attendance,
    this.completer,
  );
    
  @override
  List<Object?> get props => [attendance, completer];
}