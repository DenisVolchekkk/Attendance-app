part of 'autorize_bloc.dart';

abstract class AutorizeEvent extends Equatable {
  const AutorizeEvent();

  @override
  List<Object> get props => [];
}

class Autorize extends AutorizeEvent {
  const Autorize({
    required this.name,
    required this.password,
    this.completer,
  });

  final String name;
  final String password;
  final Completer? completer;
  @override
  List<Object> get props => [name, password];
}
class Register extends AutorizeEvent {
  const Register({
    required this.firstName,
    required this.lastName,
    required this.fatherName,
    required this.email,
    required this.password,
    required this.confirmPassword,
    required this.clientUri,
    this.completer,
  });

  final String firstName;
  final String lastName;
  final String fatherName;
  final String email;
  final String password;
  final String confirmPassword;
  final String clientUri;
  final Completer? completer;

  @override
  List<Object> get props => [firstName, lastName, fatherName, email, password, confirmPassword, clientUri];
}