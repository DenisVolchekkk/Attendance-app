import 'dart:async';

import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/repositories/attandance/abstract_attandance_repository.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:talker_flutter/talker_flutter.dart';

part 'attandance_list_event.dart';
part 'attandance_list_state.dart';

class AttandanceListBloc extends Bloc<AttandanceListEvent, AttandanceListState> {
  AttandanceListBloc(this.attandanceRepository) : super(AttandanceListInitial()) {
    on<LoadAttandanceList>(_load);
    on<UpdateAttendanceEvent>(_onUpdateAttendance);
  }

  final AbstractAttandanceRepository attandanceRepository;

  Future<void> _onUpdateAttendance(
    UpdateAttendanceEvent event,
    Emitter<AttandanceListState> emit,
  ) async {
    try {
      if (state is! AttandanceListLoaded) {
        emit(AttandanceListLoading());
      }
      // Обновите attendance в репозитории
      await attandanceRepository.isPresentStudent(event.attendance);
      
      final updatedAttandanceList = await attandanceRepository.getAttandanceList();
      // 3. Вызываем emit с новым состоянием
      emit(AttandanceListLoaded(attandanceList: updatedAttandanceList));
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
      if (state is! AttandanceListLoaded) {
        emit(AttandanceListLoading());
      }
      final attandanceList = await attandanceRepository.getAttandanceList() ;
      emit(AttandanceListLoaded(attandanceList: attandanceList));
    } catch (e, st) {
      emit(AttandanceListLoadingFailure(exception: e));
      GetIt.I<Talker>().handle(e, st);
    } finally {
      event.completer?.complete();
    }
  }

  @override
  void onError(Object error, StackTrace StackTrace) {
    super.onError(error, StackTrace);
    GetIt.I<Talker>().handle(error, StackTrace);
  }
}
