import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:get_it/get_it.dart';
import 'package:student_app/repositories/models/models.dart';
import 'package:student_app/features/schedule_list/bloc/schedule_list_bloc.dart';
import 'package:auto_route/auto_route.dart';
import 'package:student_app/repositories/schedules/abstract_schedule_repository.dart';
import 'package:student_app/router/router.dart';
import 'dart:async';

import '../widgets/widgets.dart';

@RoutePage()
class ScheduleScreen extends StatefulWidget {
  const ScheduleScreen({super.key});

  @override
  State<ScheduleScreen> createState() => _ScheduleScreenState();
}

class _ScheduleScreenState extends State<ScheduleScreen> {
  final _scheduleListBloc =
      ScheduleListBloc(GetIt.I<AbstractScheduleRepository>());
  int? _selectedDayNumber;
  String? _selectedGroupName;

  @override
  void initState() {
    _loadSchedules();
    super.initState();
  }

  void _loadSchedules() {
    _scheduleListBloc.add(LoadScheduleList(
      day: _selectedDayNumber,
      groupName: _selectedGroupName,
    ));
  }



  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text(
          _selectedDayNumber != null || _selectedGroupName != null
              ? 'Расписание${_selectedDayNumber != null ? ' (${DayUtils.getDayName(_selectedDayNumber!)})' : ''}${_selectedGroupName != null ? ' - $_selectedGroupName' : ''}'
              : 'Расписание',
        ),
      ),
      drawer: CustomDrawer(
        theme: theme,
        onHomeTap: () => context.router.popUntilRoot(),
        onSettingsTap: () {},
      ),
      body: RefreshIndicator(
        onRefresh: () async {
          final completer = Completer();
          _scheduleListBloc.add(LoadScheduleList(
            day: _selectedDayNumber,
            groupName: _selectedGroupName,
            completer: completer,
          ));
          return completer.future;
        },
        child: BlocBuilder<ScheduleListBloc, ScheduleListState>(
          bloc: _scheduleListBloc,
          builder: (context, state) {
            if (state is ScheduleListLoading) {
              return const Center(child: CircularProgressIndicator());
            }

            if (state is ScheduleListLoadingFailure) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text('Ошибка загрузки: ${state.exception}'),
                    const SizedBox(height: 16),
                    ElevatedButton(
                      onPressed: _loadSchedules,
                      child: const Text('Повторить'),
                    ),
                  ],
                ),
              );
            }

            if (state is ScheduleListLoaded) {
                return ScheduleTable(
    schedules: state.scheduleList,
    selectedDayNumber: _selectedDayNumber,
    selectedGroupName: _selectedGroupName,
    onItemTap: _navigateToAttendanceScreen,
  );
            }

            return const Center(child: Text('Нажмите кнопку загрузки'));
          },
        ),
      ),
      floatingActionButton:
          _selectedDayNumber != null || _selectedGroupName != null
              ? FloatingActionButton(
                  onPressed: () {
                    setState(() {
                      _selectedDayNumber = null;
                      _selectedGroupName = null;
                      _loadSchedules();
                    });
                  },
                  child: const Icon(Icons.clear),
                  tooltip: 'Сбросить фильтры',
                )
              : FloatingActionButton(
                  onPressed: _loadSchedules,
                  child: const Icon(Icons.refresh),
                  tooltip: 'Обновить',
                ),
    );
  }
void _navigateToAttendanceScreen(BuildContext context, Schedule schedule) {
context.router.push(AttendanceRoute(
  day: schedule.dayOfWeek,
  groupName: schedule.group.name,
  discipline: schedule.discipline.name,
  time: schedule.startTime
));
}
}