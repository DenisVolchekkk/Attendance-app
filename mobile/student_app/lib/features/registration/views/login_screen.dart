import 'dart:async';

import 'package:flutter/material.dart';
import 'package:auto_route/auto_route.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/features/registration/bloc/autorize_bloc.dart';
import 'package:student_app/repositories/autorize/abstract_autorize_repository.dart';
import 'package:student_app/router/router.dart';

@RoutePage()
class LoginScreen extends StatefulWidget {
  const LoginScreen({
    super.key,
  });

  @override
  State<LoginScreen> createState() => _LoginScreenState();
}

class _LoginScreenState extends State<LoginScreen> {
  final TextEditingController usernameController = TextEditingController();
  final TextEditingController passwordController = TextEditingController();

  final _autorizeBloc = AutorizeBloc(
    GetIt.I<AbstractAutorizeRepository>(),
  );

  @override
  void dispose() {
    usernameController.dispose();
    passwordController.dispose();
    _autorizeBloc.close();
    super.dispose();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(title: Text('Авторизация')),
      body: RefreshIndicator(
        onRefresh: () async {
          final completer = Completer();
          return completer.future;
        },
        child: BlocConsumer<AutorizeBloc, AutorizeState>(
          bloc: _autorizeBloc,
          listener: (context, state) {
            if (state is AutorizeLoaded) {
              AutoRouter.of(context).push(AttendanceRoute());
            }
          },
          builder: (context, state) {
            if ((state is AutorizeInitial) || (state is AutorizeLoaded)) {
              return Padding(
                padding: const EdgeInsets.all(16.0),
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    TextField(
                      style: theme.textTheme.bodyMedium,
                      controller: usernameController,
                      decoration:
                          InputDecoration(labelText: 'Имя пользователя', labelStyle: theme.inputDecorationTheme.labelStyle),
                    ),
                    TextField(
                      style: theme.textTheme.bodyMedium,
                      controller: passwordController,
                      decoration: InputDecoration(labelText: 'Пароль', labelStyle: theme.inputDecorationTheme.labelStyle),
                      obscureText: true,
                    ),
                    SizedBox(height: 20),
                    if (state is AutorizeLoading)
                      CircularProgressIndicator()
                    else
                      ElevatedButton(
                        onPressed: () {
                          _autorizeBloc.add(Autorize(
                            name: usernameController.text,
                            password: passwordController.text,
                          ));
                        },
                        child: Text('Войти',),
                      ),
                    TextButton(
                      onPressed: () {
                        AutoRouter.of(context).push(RegistrationRoute());
                      },
                      child: Text('Нет аккаунта? Зарегистрируйтесь',),
                    ),
                  ],
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
                        _autorizeBloc.add(Autorize(
                          name: usernameController.text,
                          password: passwordController.text,
                        ));
                      },
                      child: const Text('Попробовать снова'),
                    ),
                  ],
                ),
              );
            }else {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  crossAxisAlignment: CrossAxisAlignment.center,
                  children: [
                    Text(
                      'Something went wrong',
                      style: theme.textTheme.headlineMedium,
                    ),
                    Text(
                      'Please try againg later',
                      style: theme.textTheme.labelSmall?.copyWith(fontSize: 16),
                    ),
                    const SizedBox(height: 30),
                    TextButton(
                      onPressed: () {
                        AutoRouter.of(context).push(LoginRoute());
                      },
                      child: const Text('Try againg'),
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
