import '../models/models.dart';

abstract class AbstractScheduleRepository {
  Future<List<Schedule>> getScheduleList();
}