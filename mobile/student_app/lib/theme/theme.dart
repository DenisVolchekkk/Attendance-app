import 'package:flutter/material.dart';


final dartTheme = ThemeData(
  scaffoldBackgroundColor: const Color.fromARGB(255, 40, 39, 39),
  primarySwatch: Colors.yellow,
  colorScheme: ColorScheme.fromSeed(seedColor:  Colors.yellow),
  dividerColor: Colors.white,
  appBarTheme: const AppBarTheme(
    elevation: 0,
    backgroundColor:  Color.fromARGB(255, 40, 39, 39),
    titleTextStyle: TextStyle(color: Colors.white, fontSize: 20, fontWeight: FontWeight.w700),
    centerTitle: true,
  ),
  inputDecorationTheme: const InputDecorationTheme(
    labelStyle: TextStyle(color: Colors.white), // Белый цвет для labelText
  ),
  listTileTheme: ListTileThemeData(iconColor: Colors.white),
  textTheme:  TextTheme(
    bodyMedium: TextStyle(
      color: Colors.white,
      fontWeight: FontWeight.w500,
      fontSize: 20,
    ),

    labelSmall: TextStyle(
        color: Colors.white.withValues(alpha: 0.6),
        fontWeight: FontWeight.w700,
        fontSize: 14,
    ),
    
  ),
  
  useMaterial3: true,

);