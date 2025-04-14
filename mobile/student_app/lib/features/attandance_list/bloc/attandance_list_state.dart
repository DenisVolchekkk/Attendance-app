part of 'attandance_list_bloc.dart';

abstract class AttandanceListState extends Equatable {
  const AttandanceListState();
}

class AttandanceListInitial extends AttandanceListState {
  @override
  List<Object?> get props => [];
}

class AttandanceListLoading extends AttandanceListState {
  @override
  List<Object?> get props => [];
}

class AttandanceListLoaded extends AttandanceListState {
  final List<Attendance> attandanceList;
  final int? selectedDay;
  final String? selectedGroup;
  final String? selectedDiscipline;
  final TimeOfDay? selectedTime;
  final DateTime? selectedDate;
  
  const AttandanceListLoaded({
    required this.attandanceList,
    this.selectedDay,
    this.selectedGroup,
    this.selectedDiscipline,
    this.selectedTime,
    this.selectedDate,
  });

  @override
  List<Object?> get props => [
    attandanceList, 
    selectedDay, 
    selectedGroup,
    selectedDiscipline,
    selectedTime,
    selectedDate,
  ];
}


class AttandanceListLoadingFailure extends AttandanceListState {
  final Object? exception;
  
  const AttandanceListLoadingFailure({
    this.exception,
  });
  
  @override
  List<Object?> get props => [exception];
}

class AttendanceUpdated extends AttandanceListState {
  final Attendance attendance;
  
  const AttendanceUpdated(this.attendance);
  
  @override
  List<Object?> get props => [attendance];
}