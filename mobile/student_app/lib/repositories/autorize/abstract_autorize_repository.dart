abstract class AbstractAutorizeRepository {
  Future<Map<String, dynamic>> authentication(String email, String password);
  Future<Map<String, dynamic>> register( String firstName, String lastName, String fatherName, String email, String password, String confirmPassword,String clientUri);

}