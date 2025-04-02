
part of 'autorize_bloc.dart';

abstract class AutorizeState extends Equatable{

}

class AutorizeInitial extends AutorizeState{
  @override
  // TODO: implement props
  List<Object?> get props => [];

}

class AutorizeLoading extends AutorizeState {
  @override
  // TODO: implement props
  List<Object?> get props => [];

}

class AutorizeLoaded extends AutorizeState {
  
  AutorizeLoaded({
    required this.token,
  });
  
  final String token;
  @override
  List<Object?> get props => [token];
}

class AutorizeLoadingFailure extends AutorizeState {
  AutorizeLoadingFailure({
    this.exception,
  });

  final Object? exception;
  
  @override
  // TODO: implement props
  List<Object?> get props => [exception];

}
class RegistrationLoaded extends AutorizeState {
  RegistrationLoaded({
    this.isRegistered = false,
    this.errorMessage,
  });

  final bool isRegistered;
  final String? errorMessage;

  @override
  List<Object?> get props => [isRegistered, errorMessage];

}