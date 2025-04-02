import 'package:dio/dio.dart';
import 'package:student_app/repositories/autorize/abstract_autorize_repository.dart';

class AutorizeRepository implements AbstractAutorizeRepository {
  AutorizeRepository({
    required this.dio,
  });

  final Dio dio;
  
  @override
  Future<Map<String, dynamic>> authentication(String email, String password) async {
    final url = 'http://192.168.48.51:5183/api/Accounts/authenticate';
    final headers = {
      'accept': '*/*',
      'Content-Type': 'application/json',
    };
    final body = {
      'email': email,
      'password': password,
    };

    try {
      final response = await dio.post(url, data: body, options: Options(headers: headers));

      if (response.statusCode == 200) {
        return response.data;
      } else {
        throw Exception('Failed to authenticate user');
      }
    } catch (e) {
      throw Exception('Failed to authenticate user: $e');
    }
  }

  @override
  Future<Map<String, dynamic>> register(
     String firstName,
     String lastName,
     String fatherName,
     String email,
     String password,
     String confirmPassword,
     String clientUri,
  ) async {
    final url = 'http://192.168.48.51:5183/api/Accounts/register';
    final headers = {
      'accept': '*/*',
      'Content-Type': 'application/json',
    };
    final body = {
      'firstName': firstName,
      'lastName': lastName,
      'fatherName': fatherName,
      'email': email,
      'password': password,
      'confirmPassword': confirmPassword,
      'clientUri': clientUri,
    };

    try {
      final response = await dio.post(url, data: body, options: Options(headers: headers));

      if (response.statusCode == 201) {
        return {'isRegistered': true};
      } else {
        throw Exception('Failed to register user');
      }
    } catch (e) {
      throw Exception('Failed to register user: $e');
    }
  }
}