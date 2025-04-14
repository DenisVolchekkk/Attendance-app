import 'dart:async';

import 'package:flutter/material.dart';
import 'package:flutter_bloc/flutter_bloc.dart';
import 'package:student_app/repositories/attandance/abstract_attandance_repository.dart';
import 'package:student_app/features/attandance_list/bloc/attandance_list_bloc.dart';
import 'package:auto_route/auto_route.dart';
import 'package:get_it/get_it.dart';

import '../widgets/widgets.dart';

@RoutePage()
class AttendanceScreen extends StatefulWidget {
  const AttendanceScreen({
    super.key,
    this.day,
    this.groupName,
    this.discipline,
    this.time,
    this.date,
  });

  final int? day;
  final String? groupName;
  final String? discipline;
  final TimeOfDay? time;
  final DateTime? date;

  @override
  State<AttendanceScreen> createState() => _AttendanceScreenState();
}

class _AttendanceScreenState extends State<AttendanceScreen> {
  final _futureAttendancesBloc =
      AttandanceListBloc(GetIt.I<AbstractAttandanceRepository>());
  int? _selectedDayNumber;
  String? _selectedGroupName;
  String? _selectedDiscipline;
  TimeOfDay? _selectedTime;
  DateTime? _selectedDate;
  List<String> _availableGroups = [];
  List<String> _availableDisciplines = [];

  @override
  void initState() {
    _selectedDayNumber = widget.day;
    _selectedGroupName = widget.groupName;
    _selectedDiscipline = widget.discipline;
    _selectedTime = widget.time;
    _selectedDate = widget.date;
    _loadAttendances();
    _fetchAvailableGroups();
    _fetchAvailableDisciplines();
    super.initState();
  }

  void _loadAttendances() {
    _futureAttendancesBloc.add(LoadAttandanceList(
      day: _selectedDayNumber,
      groupName: _selectedGroupName,
      discipline: _selectedDiscipline,
      attendanceTime: _selectedTime,
      attendanceDate: _selectedDate,
    ));
  }

  Future<void> _fetchAvailableGroups() async {
    try {
      final groups = await GetIt.I<AbstractAttandanceRepository>().getTeacherGroups();
      setState(() {
        _availableGroups = groups;
      });
    } catch (e) {
      // Handle error
    }
  }

  Future<void> _fetchAvailableDisciplines() async {
    try {
      final disciplines = await GetIt.I<AbstractAttandanceRepository>().getTeacherDisciplines();
      setState(() {
        _availableDisciplines = disciplines;
      });
    } catch (e) {
      // Handle error
    }
  }

  @override
  Widget build(BuildContext context) {
    final theme = Theme.of(context);

    return Scaffold(
      appBar: AppBar(
        title: Text(
          _selectedDayNumber != null || 
          _selectedGroupName != null ||
          _selectedDiscipline != null ||
          _selectedTime != null ||
          _selectedDate != null
              ? 'Attendance${_selectedDayNumber != null ? ' (${DayUtils.getDayName(_selectedDayNumber!)})' : ''}${_selectedGroupName != null ? ' - $_selectedGroupName' : ''}${_selectedDiscipline != null ? ' - $_selectedDiscipline' : ''}${_selectedTime != null ? ' - ${_selectedTime!.format(context)}' : ''}${_selectedDate != null ? ' - ${_selectedDate!.day}.${_selectedDate!.month}.${_selectedDate!.year}' : ''}'
              : 'All Attendances',
        ),
      ),
      drawer: CustomDrawer(
        theme: theme,
        selectedDayNumber: _selectedDayNumber,
        selectedGroupName: _selectedGroupName,
        selectedDiscipline: _selectedDiscipline,
        selectedTime: _selectedTime,
        selectedDate: _selectedDate,
        onDaySelected: (dayNumber) {
          setState(() {
            _selectedDayNumber = dayNumber;
            _loadAttendances();
          });
        },
        onGroupSelected: (groupName) {
          setState(() {
            _selectedGroupName = groupName;
            _loadAttendances();
          });
        },
        onDisciplineSelected: (discipline) {
          setState(() {
            _selectedDiscipline = discipline;
            _loadAttendances();
          });
        },
        onTimeSelected: (timeString) {
          setState(() {
            _selectedTime = timeString; // Теперь храним строку
            _loadAttendances();
          });
        },
        onDateSelected: (date) {
          setState(() {
            _selectedDate = date;
            _loadAttendances();
          });
        },
        onHomeTap: () => context.router.popUntilRoot(),
        onSettingsTap: () {},
        availableGroups: _availableGroups,
        availableDisciplines: _availableDisciplines,
      ),
      body: RefreshIndicator(
        onRefresh: () async {
          final completer = Completer();
          _futureAttendancesBloc.add(LoadAttandanceList(
            day: _selectedDayNumber,
            groupName: _selectedGroupName,
            discipline: _selectedDiscipline,
            attendanceTime: _selectedTime,
            attendanceDate: _selectedDate,
            completer: completer,
          ));
          return completer.future;
        },
        child: BlocBuilder<AttandanceListBloc, AttandanceListState>(
          bloc: _futureAttendancesBloc,
          builder: (context, state) {
            if (state is AttandanceListLoaded) {
              final attendances = state.attandanceList;

              if (attendances.isEmpty) {
                return Center(
                  child: Text(
                    _selectedDayNumber != null || 
                    _selectedGroupName != null ||
                    _selectedDiscipline != null ||
                    _selectedTime != null ||
                    _selectedDate != null
                        ? 'No attendances${_selectedDayNumber != null ? ' for ${DayUtils.getDayName(_selectedDayNumber!)}' : ''}${_selectedGroupName != null ? ' in $_selectedGroupName' : ''}${_selectedDiscipline != null ? ' for $_selectedDiscipline' : ''}${_selectedTime != null ? ' at ${_selectedTime!.format(context)}' : ''}${_selectedDate != null ? ' on ${_selectedDate!.day}.${_selectedDate!.month}.${_selectedDate!.year}' : ''}'
                        : 'No attendances available',
                  ),
                );
              }

              return ListView.separated(
                padding: const EdgeInsets.only(top: 16),
                itemCount: attendances.length,
                separatorBuilder: (context, index) => const Divider(),
                itemBuilder: (context, i) {
                  final attendance = attendances[i];
                  return AttandanceTile(
                    attendance: attendance,
                    bloc: _futureAttendancesBloc,
                  );
                },
              );
            }
            if (state is AttandanceListLoadingFailure) {
              return Center(
                child: Column(
                  mainAxisAlignment: MainAxisAlignment.center,
                  children: [
                    Text(
                      'Something went wrong',
                      style: theme.textTheme.headlineMedium,
                    ),
                    const SizedBox(height: 10),
                    TextButton(
                      onPressed: _loadAttendances,
                      child: const Text('Try again'),
                    ),
                  ],
                ),
              );
            }
            return const Center(child: CircularProgressIndicator());
          },
        ),
      ),
      floatingActionButton: _selectedDayNumber != null || 
          _selectedGroupName != null ||
          _selectedDiscipline != null ||
          _selectedTime != null ||
          _selectedDate != null
          ? FloatingActionButton(
              onPressed: () {
                setState(() {
                  _selectedDayNumber = null;
                  _selectedGroupName = null;
                  _selectedDiscipline = null;
                  _selectedTime = null;
                  _selectedDate = null;
                  _loadAttendances();
                });
              },
              child: const Icon(Icons.clear),
              tooltip: 'Clear filters',
            )
          : null,
    );
  }
}