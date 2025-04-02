import 'dart:async';

import 'package:equatable/equatable.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:shared_preferences/shared_preferences.dart';
import 'package:student_app/repositories/autorize/abstract_autorize_repository.dart';
import 'package:talker_flutter/talker_flutter.dart';

part 'autorize_event.dart';
part 'autorize_state.dart';

class AutorizeBloc extends Bloc<AutorizeEvent, AutorizeState> {
  AutorizeBloc(this.AutorizeRepository) : super(AutorizeInitial()) {
    on<Autorize>(autorize);
    on<Register>(register);
  }


  final AbstractAutorizeRepository AutorizeRepository;

  
  Future<void> autorize(    
    Autorize event,
    Emitter<AutorizeState> emit,) async {
    try {
      final response = await AutorizeRepository.authentication(event.name, event.password);
      
      if (response['isAuthSuccessful'] == true) {
        final token = response['token'];

        final prefs = await SharedPreferences.getInstance();
        await prefs.setString('auth_token', token);
        
        emit(AutorizeLoaded(token: token));
      } else {
        emit(AutorizeLoadingFailure(exception: response['errorMessage']));
      }
    } catch (e) {
      emit(AutorizeLoadingFailure(exception: e.toString()));
    }
    finally {
      event. completer?.complete();
    }
  }


  Future<void> register(Register event, Emitter<AutorizeState> emit) async {
    try {
      final response = await AutorizeRepository.register(
        event.firstName,
        event.lastName,
        event.fatherName,
        event.email,
        event.password,
        event.confirmPassword,
        event.clientUri,
      );

      if (response['isRegistered'] == true) {
        emit(RegistrationLoaded(isRegistered: true));
      } else {
        emit(RegistrationLoaded(errorMessage: response['errorMessage']));}

}       catch (e) {
      emit(AutorizeLoadingFailure(exception: e.toString()));}
      finally {
      event.completer?.complete();
    }
  }

  @override
  void onError(Object error, StackTrace StackTrace) {
    super.onError(error, StackTrace);
    GetIt.I<Talker>().handle(error, StackTrace);
  }
}
