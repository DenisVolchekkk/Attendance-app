import 'package:flutter/material.dart';
import 'package:auto_route/auto_route.dart';
@RoutePage()
class GuideScreen extends StatelessWidget {
  const GuideScreen({super.key});

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: AppBar(
        title: const Text('РУКОВОДСТВО ПОЛЬЗОВАТЕЛЯ'),
        centerTitle: true,
      ),
      body: SingleChildScrollView(
        padding: const EdgeInsets.all(16.0),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Приложение предназначено для учета посещаемости студентов.',
              style: TextStyle(fontSize: 16),
            ),
            const SizedBox(height: 20),
            _buildImageSection(
              'https://i.imgur.com/iSGsrQI.png', // Replace with actual image URL
              'На этой странице пользователь должен ввести свой логин и пароль и нажать войти',
            ),
            const SizedBox(height: 20),
            const Text(
              
              'Для успешной регистрации важно заполнить все поля и ввести существующий '
              'адрес электронной почты и ввести пароль в котором содержатся больше 8 '
              'символов, латинская строчная и прописная буквы, а также 1 специальный '
              'символ. После этого пользователю на почту придет ссылка, перейдя по '
              'которой он подтвердить почту и перейти к главной страницу.',
              style: TextStyle(fontSize: 16),
            ),
            _buildImageSection(
              'https://i.imgur.com/tyyxmz6.png', // Replace with actual image URL
              'Cтраница регистрации',
            ),
            const SizedBox(height: 20),
            const Text(
              'Правильно введя логин и пароль, пользователь попадет на страницу '
              'посещений. И увидит все записи о посещении и нажав на чекбокс можно '
              'отметить присутствовал ли студент. Старости видят только посещения своей '
              'группы, а преподаватель только те записи, в расписании которых он указан',
              style: TextStyle(fontSize: 16),
            ),
            const SizedBox(height: 20),
            _buildImageSection(
              'https://i.imgur.com/24ZSMZp.png', // Replace with actual image URL
              'Страница посещений',
            ),
            const SizedBox(height: 20),
            _buildImageSection(
              'https://i.imgur.com/2UVDysV.png', // Replace with actual image URL
              'Главное меню приложения',
            ),
            const SizedBox(height: 20),
            const Text(
              'В этом окне пользователь может указать параметры для отображаемых '
              'посещений. Перейти на страницу посещений, гайда или расписания.',
              style: TextStyle(fontSize: 16),
            ),
            const SizedBox(height: 20),
            _buildImageSection(
              'https://i.imgur.com/nQTVATO.png', // Replace with actual image URL
              'Страница расписания',
            ),
            const SizedBox(height: 20),
            const Text(
              'На этой странице указано расписание пользователя старосты или '
              'преподавателя и нажав строчку в этом расписании пользователь увидит '
              'страницу посещений с соответствующими параметрами.',
              style: TextStyle(fontSize: 16),
            ),
            const SizedBox(height: 20),
            _buildImageSection(
              'https://i.imgur.com/fw7ssll.png', // Replace with actual image URL
              'Пример страницы посещений',
            ),
            const SizedBox(height: 20),
          ],
        ),
      ),
    );
  }

  Widget _buildImageSection(String imageUrl, String description) {
    return Column(
      children: [
        Image.network(
          imageUrl,
          width: 300,
          height: 200,
          fit: BoxFit.contain,
          loadingBuilder: (context, child, loadingProgress) {
            if (loadingProgress == null) return child;
            return Center(
              child: CircularProgressIndicator(
                value: loadingProgress.expectedTotalBytes != null
                    ? loadingProgress.cumulativeBytesLoaded / loadingProgress.expectedTotalBytes!
                    : null,
              ),
            );
          },
          errorBuilder: (context, error, stackTrace) {
            return const Icon(Icons.error, size: 50, color: Colors.red);
          },
        ),
        const SizedBox(height: 8),
        Text(
          description,
          style: const TextStyle(fontStyle: FontStyle.italic),
          textAlign: TextAlign.center,
        ),
      ],
    );
  }
}