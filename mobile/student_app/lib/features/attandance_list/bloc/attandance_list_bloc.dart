import 'dart:async';
import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/repositories/attandance/abstract_attandance_repository.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:talker_flutter/talker_flutter.dart';

part 'attandance_list_event.dart';
part 'attandance_list_state.dart';

class AttandanceListBloc extends Bloc<AttandanceListEvent, AttandanceListState> {
  final AbstractAttandanceRepository attandanceRepository;

  AttandanceListBloc(this.attandanceRepository) : super(AttandanceListInitial()) {
    on<LoadAttandanceList>(_load);
    on<UpdateAttendanceEvent>(_onUpdateAttendance);
  }

  Future<void> _onUpdateAttendance(
    UpdateAttendanceEvent event,
    Emitter<AttandanceListState> emit,
  ) async {
    try {
      if (state is! AttandanceListLoaded) {
        emit(AttandanceListLoading());
      }
      
      await attandanceRepository.isPresentStudent(event.attendance);
      
      final currentState = state as AttandanceListLoaded;
      final updatedAttandanceList = await attandanceRepository.getAttandanceList(day: currentState.selectedDay, 
      groupName: currentState.selectedGroup, discipline: currentState.selectedDiscipline, 
      attendanceDate: currentState.selectedDate, attendanceTime: currentState.selectedTime);
      
      emit(AttandanceListLoaded(
        attandanceList: updatedAttandanceList,
        selectedDay: currentState.selectedDay,
        selectedGroup: currentState.selectedGroup, 
        selectedDiscipline: currentState.selectedDiscipline, 
        selectedDate: currentState.selectedDate, 
        selectedTime: currentState.selectedTime
      ));
    } catch (e, st) {
      emit(AttandanceListLoadingFailure(exception: e));
      GetIt.I<Talker>().handle(e, st);
    } finally {
      event.completer?.complete();
    }
  }

  Future<void> _load(
    LoadAttandanceList event,
    Emitter<AttandanceListState> emit,
  ) async {
    try {
      emit(AttandanceListLoading());
      
      final attandanceList = await attandanceRepository.getAttandanceList(day: event.day, groupName: event.groupName,
      discipline: event.discipline, 
      attendanceDate: event.attendanceDate, attendanceTime: event.attendanceTime);
      
      emit(AttandanceListLoaded(
        attandanceList: attandanceList,
        selectedDay: event.day,
        selectedGroup: event.groupName,
        selectedDiscipline: event.discipline, 
        selectedDate: event.attendanceDate, 
        selectedTime: event.attendanceTime
      ));
    } catch (e, st) {
      emit(AttandanceListLoadingFailure(exception: e));
      GetIt.I<Talker>().handle(e, st);
    } finally {
      event.completer?.complete();
    }
  }

  @override
  void onError(Object error, StackTrace stackTrace) {
    super.onError(error, stackTrace);
    GetIt.I<Talker>().handle(error, stackTrace);
  }
}