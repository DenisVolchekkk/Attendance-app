part of 'schedule_list_bloc.dart';

abstract class ScheduleListState extends Equatable {
  const ScheduleListState();
}

class ScheduleListInitial extends ScheduleListState {
  @override
  List<Object?> get props => [];
}

class ScheduleListLoading extends ScheduleListState {
  @override
  List<Object?> get props => [];
}

class ScheduleListLoaded extends ScheduleListState {
  final List<Schedule> scheduleList;
  
  const ScheduleListLoaded({
    required this.scheduleList,
  });

  @override
  List<Object?> get props => [scheduleList];
}

class ScheduleListLoadingFailure extends ScheduleListState {
  final Object? exception;
  
  const ScheduleListLoadingFailure({
    this.exception,
  });
  
  @override
  List<Object?> get props => [exception];
}