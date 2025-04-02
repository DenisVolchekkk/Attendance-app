part of 'attandance_list_bloc.dart';

abstract class AttandanceListState extends Equatable{

}

class AttandanceListInitial extends AttandanceListState{
  @override
  // TODO: implement props
  List<Object?> get props => [];

}

class AttandanceListLoading extends AttandanceListState {
  @override
  // TODO: implement props
  List<Object?> get props => [];

}

class AttandanceListLoaded extends AttandanceListState {
  
  final List<Attendance> attandanceList;
  final int? selectedDayOfWeek;
  final String? selectedDiscipline;

  AttandanceListLoaded({
    required this.attandanceList,
    this.selectedDayOfWeek,
    this.selectedDiscipline,
  });
  @override
  List<Object?> get props => [attandanceList, selectedDayOfWeek, selectedDiscipline];
}

class AttandanceListLoadingFailure extends AttandanceListState {
  AttandanceListLoadingFailure({
    this.exception,
  });

  final Object? exception;
  
  @override
  // TODO: implement props
  List<Object?> get props => [exception];

}
class AttendanceUpdated extends AttandanceListState {
  final Attendance attendance;

  AttendanceUpdated(this.attendance);
  
  @override
  // TODO: implement props
  List<Object?> get props => [attendance];
}
