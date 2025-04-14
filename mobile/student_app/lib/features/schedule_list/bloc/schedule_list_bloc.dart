import 'dart:async';
import 'package:equatable/equatable.dart';
import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/repositories/schedules/abstract_schedule_repository.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:talker_flutter/talker_flutter.dart';

part 'schedule_list_event.dart';
part 'schedule_list_state.dart';

class ScheduleListBloc extends Bloc<ScheduleListEvent, ScheduleListState> {
  final AbstractScheduleRepository scheduleRepository;

  ScheduleListBloc(this.scheduleRepository) : super(ScheduleListInitial()) {
    on<LoadScheduleList>(_load);
  }

  Future<void> _load(
    LoadScheduleList event,
    Emitter<ScheduleListState> emit,
  ) async {
    try {
      emit(ScheduleListLoading());
      
      final scheduleList = await scheduleRepository.getScheduleList(
      );
      
      emit(ScheduleListLoaded(
        scheduleList: scheduleList,
      ));
    } catch (e, st) {
      emit(ScheduleListLoadingFailure(exception: e));
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