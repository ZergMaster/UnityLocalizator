   M S B A M L   `   `   `  ����F  BLocalizator, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null)clr-namespace:LocalizatorLocalizator  5   	   !    Localizator.AddDialogWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35^ ZWindowsFormsIntegration, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   +localclr-namespace:Localizator     5   	     � Title$  Add localization��5   	   $	��200��6"   $	��400��6/    � 
ResizeMode$ CanResizeWithGrip=�5	   	    � ShowInTaskbar .   � WindowStartupLocation$ CenterOwner=�5   	   .��5      �  � 	Resources 5   
    �� 
TargetType �� Property �� Value �� Triggers	 P� Property
 P� Value �� RelativeSource �� Path��%�  '�       '�� H     '�� �     '�� �    '�� 5    �� 5      " �6   .�5      ��  � ��$ 10q�6+   5      �� 5      " ��6   .�5      ��  � ��$ 0,3,5,5q�6+   �� 5       � ��$ 0,0,0,5q�6,   5      �� 5      " ��6   .�5      ��  � ��$ 0,0,0,5q�6+    5      P� 5      	 �@=�HasError
 . . �5      ��  �@�ToolTip 64   ��  �� *Self+$) #(Validation.Errors)[0].ErrorContent�5      5      �� 5      " ��6   .�5      ��  �@��Orientation$ 
Horizontal=�60   �� 5        � ��$ Right=�68   5!      �� 5"      " ��6   .�5#      ��  � ��$ 70��6*   �� 5$       � ��$ 25��6+   �� 5%       � ��$ 5,0,0,0q�6+   5&      5'       � ColumnDefinitions 5)   
   �� 5*      $
��Auto �6   �� 5+       � RowDefinitions 5.   
   �� 5/      $
��Auto �6   �� 50      $
��Auto �6   �� 51      $
��Auto �6   .+�54   
   �� $��0��6   $��0��6    .��6-   	Ключ:64   �� 55   
   -   5         ��Name$ KeyBox��56      $��1��6"   $��0��62   �� 58   
   $��0��6   $��1��6    .��6-   Значение RU:6;   �� 59   
   -   5        $ 
ValueRuBox��59      $��1��6&   $��1��66   �� 5;      $��0��6   $��2��6   .��6+   Значение EN:69   �� 5<      -   5        $ 
ValueEnBox��5<      $��1��6$   $��2��64   �� 5>   
   $��0��6   $��2��6%   $��3��69   .+�5?      �� -   5         �� 	IsDefault . .��5?   >   OK6B   �� 5@      -   5         �� IsCancel . .��5@   A   Cancel6I   5A      5D      5E      