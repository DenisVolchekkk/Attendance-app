import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/features/attandance_list/bloc/attandance_list_bloc.dart';
import 'package:student_app/repositories/attandance/abstract_attandance_repository.dart';
import '../widgets/widgets.dart';
import 'package:auto_route/auto_route.dart';

@RoutePage()
class AttendanceScreen extends StatefulWidget {
  const AttendanceScreen({
    super.key,
  });

  @override
  State<AttendanceScreen> createState() => _AttendanceScreenState();
}

class _AttendanceScreenState extends State<AttendanceScreen> {
  final _futureAttendancesBloc =
      AttandanceListBloc(GetIt.I<AbstractAttandanceRepository>());

  @override
  void initState() {
    _futureAttendancesBloc.add(LoadAttandanceList());
    super.initState();
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text('Attendance List'),
      ),
      drawer: Drawer(
        child: ListView(
          padding: EdgeInsets.zero,
          children: <Widget>[
            DrawerHeader(
              decoration: BoxDecoration(
                color: theme.primaryColor,
              ),
              child: Text(
                'Menu',
                style: TextStyle(
                  color: Colors.white,
                  fontSize: 24,
                ),
              ),
            ),
            ListTile(
              leading: Icon(Icons.home),
              title: Text('Home'),
              onTap: () {
                // Navigate to Home screen
                Navigator.pop(context); // Close the drawer
              },
            ),
            ListTile(
              leading: Icon(Icons.settings),
              title: Text('Settings'),
              onTap: () {
                // Navigate to Settings screen
                Navigator.pop(context); // Close the drawer
              },
            ),
          ],
        ),
      ),
      body: RefreshIndicator(
        onRefresh: () async {
          final completer = Completer();
          _futureAttendancesBloc.add(LoadAttandanceList(completer: completer));
          return completer.future;
        },
        child: BlocBuilder<AttandanceListBloc, AttandanceListState>(
          bloc: _futureAttendancesBloc,
          builder: (context, state) {
            if (state is AttandanceListLoaded) {
              return ListView.separated(
                padding: const EdgeInsets.only(top: 16),
                itemCount: state.attandanceList.length,
                separatorBuilder: (context, index) => const Divider(),
                itemBuilder: (context, i) {
                  final attandance = state.attandanceList[i];
                  return AttandanceTile(
                      attendance: attandance, bloc: _futureAttendancesBloc);
                },
              );
            }
            if (state is AttandanceListLoadingFailure) {
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
                        _futureAttendancesBloc.add(LoadAttandanceList());
                      },
                      child: const Text('Try againg'),
                    ),
                  ],
                ),
              );
            }
            return const Center(child: CircularProgressIndicator());
          },
        ),
      ),
    );
  }
}
