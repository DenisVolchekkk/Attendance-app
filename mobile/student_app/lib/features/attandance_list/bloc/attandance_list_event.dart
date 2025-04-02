// ignore_for_file: public_member_api_docs, sort_constructors_first
part of 'attandance_list_bloc.dart';

abstract class AttandanceListEvent extends Equatable{}

class LoadAttandanceList extends AttandanceListEvent {
  LoadAttandanceList({
    this.completer,
  });

  final Completer? completer;
  
  @override
  // TODO: implement props
  List<Object?> get props => [completer];
}
class UpdateAttendanceEvent extends AttandanceListEvent {
  final Attendance attendance;
  final Completer? completer;
  UpdateAttendanceEvent(
    this.attendance,
    this.completer,
  );
    
  @override
  // TODO: implement props
  List<Object?> get props => [attendance, completer];
}

