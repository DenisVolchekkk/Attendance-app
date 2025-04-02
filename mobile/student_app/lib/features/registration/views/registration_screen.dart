import 'dart:async';

import 'package:flutter/material.dart';
import 'package:auto_route/auto_route.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/features/registration/bloc/autorize_bloc.dart';
import 'package:student_app/repositories/autorize/abstract_autorize_repository.dart';
import 'package:student_app/router/router.dart';

@RoutePage()
class RegistrationScreen extends StatefulWidget {
  const RegistrationScreen({
    super.key,
  });

  @override
  State<RegistrationScreen> createState() => _RegistrationScreenState();
}

class _RegistrationScreenState extends State<RegistrationScreen> {
  final TextEditingController firstNameController = TextEditingController();
  final TextEditingController lastNameController = TextEditingController();
  final TextEditingController fatherNameController = TextEditingController();
  final TextEditingController emailController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();
  final TextEditingController confirmPasswordController = TextEditingController();

  final _autorizeBloc = AutorizeBloc(
    GetIt.I<AbstractAutorizeRepository>(),
  );

  @override
  void dispose() {
    firstNameController.dispose();
    lastNameController.dispose();
    fatherNameController.dispose();
    emailController.dispose();
    passwordController.dispose();
    confirmPasswordController.dispose();
    _autorizeBloc.close();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: Text('Регистрация')),
      body: RefreshIndicator(
        onRefresh: () async {
          final completer = Completer();
          return completer.future;
        },
        child: BlocConsumer<AutorizeBloc, AutorizeState>(
          bloc: _autorizeBloc,
          listener: (context, state) {
            if (state is RegistrationLoaded && state.isRegistered) {
                ScaffoldMessenger.of(context).showSnackBar(
                SnackBar(
                  content: Text('Регистрация успешна! Пожалуйста, проверьте вашу почту.'),
                  duration: Duration(seconds: 10), // Уведомление будет видно 5 секунд
                ),
              );
              AutoRouter.of(context).push(LoginRoute()); // Возврат на предыдущий экран после успешной регистрации
            }
          },
          builder: (context, state) {
            if ((state is AutorizeInitial) || (state is RegistrationLoaded && state.isRegistered)) {
              return Padding(
                padding: const EdgeInsets.all(16.0),
                child: SingleChildScrollView(
                  child: Column(
                    mainAxisAlignment: MainAxisAlignment.center,
                    children: [
                      TextField(
                        style: theme.textTheme.bodyMedium,
                        controller: firstNameController,
                        decoration: InputDecoration(
                          labelText: 'Имя',
                          labelStyle: theme.inputDecorationTheme.labelStyle,
                        ),
                      ),
                      TextField(
                        style: theme.textTheme.bodyMedium,
                        controller: lastNameController,
                        decoration: InputDecoration(
                          labelText: 'Фамилия',
                          labelStyle: theme.inputDecorationTheme.labelStyle,
                        ),
                      ),
                      TextField(
                        style: theme.textTheme.bodyMedium,
                        controller: fatherNameController,
                        decoration: InputDecoration(
                          labelText: 'Отчество',
                          labelStyle: theme.inputDecorationTheme.labelStyle,
                        ),
                      ),
                      TextField(
                        style: theme.textTheme.bodyMedium,
                        controller: emailController,
                        decoration: InputDecoration(
                          labelText: 'Email',
                          labelStyle: theme.inputDecorationTheme.labelStyle,
                        ),
                      ),
                      TextField(
                        style: theme.textTheme.bodyMedium,
                        controller: passwordController,
                        decoration: InputDecoration(
                          labelText: 'Пароль',
                          labelStyle: theme.inputDecorationTheme.labelStyle,
                        ),
                        obscureText: true,
                      ),
                      TextField(
                        style: theme.textTheme.bodyMedium,
                        controller: confirmPasswordController,
                        decoration: InputDecoration(
                          labelText: 'Повторите пароль',
                          labelStyle: theme.inputDecorationTheme.labelStyle,
                        ),
                        obscureText: true,
                      ),
                      SizedBox(height: 20),
                      if (state is AutorizeLoading)
                        CircularProgressIndicator()
                      else
                        ElevatedButton(
                          onPressed: () {
                            _autorizeBloc.add(Register(
                              firstName: firstNameController.text,
                              lastName: lastNameController.text,
                              fatherName: fatherNameController.text,
                              email: emailController.text,
                              password: passwordController.text,
                              confirmPassword: confirmPasswordController.text,
                              clientUri: 'http://192.168.48.51:5183/api/Accounts/emailconfirmation',
                            ));
                          },
                          child: Text('Зарегистрироваться'),
                        ),
                      TextButton(
                        onPressed: () {
                          AutoRouter.of(context).push(LoginRoute());
                        },
                        child: Text('Уже есть аккаунт? Войти'),
                      ),
                    ],
                  ),
                ),
              );
            } else if (state is AutorizeLoadingFailure) {
              // Отображение ошибки, если состояние - AutorizeLoadingFailure
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Text(
                      'Что-то пошло не так',
                      style: theme.textTheme.headlineMedium,
                    ),
                    Text(
                      'Ошибка: ${state.exception}', // Отображение сообщения об ошибке
                      style: theme.textTheme.labelSmall?.copyWith(fontSize: 16),
                    ),
                    const SizedBox(height: 30),
                    TextButton(
                      onPressed: () {
                        _autorizeBloc.add(Register(
                          firstName: firstNameController.text,
                          lastName: lastNameController.text,
                          fatherName: fatherNameController.text,
                          email: emailController.text,
                          password: passwordController.text,
                          confirmPassword: confirmPasswordController.text,
                          clientUri: 'http://your-client-uri.com', // Замените на ваш URI
                        ));
                      },
                      child: const Text('Попробовать снова'),
                    ),
                  ],
                ),
              );
            } else {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Text(
                      'Что-то пошло не так',
                      style: theme.textTheme.headlineMedium,
                    ),
                    Text(
                      'Пожалуйста, попробуйте позже',
                      style: theme.textTheme.labelSmall?.copyWith(fontSize: 16),
                    ),
                    const SizedBox(height: 30),
                    TextButton(
                      onPressed: () {
                        AutoRouter.of(context).push(RegistrationRoute());
                      },
                      child: const Text('Попробовать снова'),
                    ),
                  ],
                ),
              );
            }
          },
        ),
      ),
    );
  }
}