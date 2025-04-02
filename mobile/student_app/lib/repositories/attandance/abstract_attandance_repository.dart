import '../models/models.dart';

abstract class AbstractAttandanceRepository {
  Future<List<Attendance>> getAttandanceList();
  Future<void> isPresentStudent(Attendance attendance);
}