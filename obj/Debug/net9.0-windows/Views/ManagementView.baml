   M S B A M L   `   `   `  ����I  Eeatery-manager, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null;"clr-namespace:eatery_manager.Viewseatery_manager.Views  5      )    #eatery_manager.Views.ManagementView   5      W SPresentationCore, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=9.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5      R NSystem.Xaml, Version=9.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5      Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5      @d2http://schemas.microsoft.com/expression/blend/2008     5      4local"clr-namespace:eatery_manager.Views     5      .��5	      �   � ColumnDefinitions  5   
   �� 5      $��3* �6   �� 5      $��6* �6   .+�5   
   � $��0��6    � RowDefinitions 5      �� 5      $
��Auto �6    �� 5      $��* �6    .+�5      �� $��Zarządzanie aplikacją��6   $��24(�65   $��Center=�6C   $	��Top=�6`   $��	0,10,0,10q�6x   $��0��6�   �� 5      $��1��6   $	��Top=�6&   $��Center=�6>   $��	0,10,0,10q�6[   .+�5      ��  �� Orientation$ 
Horizontal=�6   $��Center=�66   $��0,0,0,10q�6S   .+�5      �� $��Usługa WWW:��6    $��Center=�63   $��16(�6N   $��0,0,10,0q�6\   d� 5      -   5        $��60��5   #   $��40��6.   ��5      �� 5        �� 
TargetType" d�6.   .R�5!   "   ��  ��Name$ border��6)   
���  �����#999$��1q�6]    �� CornerRadius$ 4��6q   ._�5"   &   ��  ��Name$ contentPresenter��67   $��Center=�6Q   $��Center=�6n   5#   #    �� Triggers 5$   "   P� 5%   &    P� Property �@d�	IsChecked	 P� Value$
	 TrueY�6C   . �5&   *   �� 
 �� 
TargetName$
 border��61    �� Property � �� �� Value
 � � �6[   �� 5'   *   $
 contentPresenter��61    � ��$ ON��6b   5(   '   P� 5)   &    �@d�	IsChecked$	 FalseY�6C   . �5*   *   �� $
 border��61    � ��
 �  ��6[   �� 5+   *   $
 contentPresenter��61    � ��$	 OFF��6b   5,   '   P� 5-   &    �@J�IsMouseOver$
	 True��6E   . �5.   *   ��  �@�Cursor$
 Hand��6C   5/   '   ^� 50   &    ^� 
Conditions 51   *   �� 52   .    �� Binding 68   ��  �� ElementName$ HttpCheckBox�� �� Path$ 	IsChecked� �� Value	 False6u   �� 53   .    68   �� $ HttpsCheckBox��$ 	IsChecked�	 False6v   .0�55   *   ��  � {�$ False��6F   �� 56   *    �@d�	IsChecked$ FalseY�6F   57   '   P� 58   &    � {�$	 False��6C   . �59   *   �� $
 border��61    �@J�Opacity$	 0.5Q�6X   5:   '   5<      5>      5?      �� 5B      $ 
Horizontal=�6   $��Center=�66   $��0,10,0,0q�6S   .+�5C      �� -   5         ��Name$ HttpCheckBox��5C      $
��HTTP��65   $��Center=�6D   $��0,0,15,0q�6_    d� 	IsChecked$
 TrueY�6q   �� 5D      -   5        $ HttpsCheckBox��5D      $��HTTPS��66   $��Center=�6F   5E      �� 5H      $��0,15,0,0q�6   .+�5I      �� $ 
Horizontal=�6!   $��0,5,0,5q�6:   {�6K   �� $ HttpCheckBox��$ 	IsChecked�.+�5J      �� $��
Port HTTP:��6$   $��70��66   $��Center=�6A   �� 5K      -   5         ��Name$ HttpPortTextBox��5K   "   $��60��6;    �� 	MaxLength$ 5��6F   $
��8080��6T   $��Center=�6`   5L      �� 5M      $ 
Horizontal=�6!   $��0,5,0,5q�6:   {�6K   �� $ HttpsCheckBox��$ 	IsChecked�.+�5N      �� $��Port HTTPS:��6$   $��70��67   $��Center=�6B   �� 5O      -   5        $ HttpsPortTextBox��5O   "   $��60��6<   $ 5��6G   $
��8443��6U   $��Center=�6a   5P      5Q      �� 5T      $��0,20,0,0q�6   .+�5U      �� $ 
Horizontal=�6!   .+�5V      �� $��Wybierz miejsce plików:��6$   $��Center=�6C   $��0,0,10,0q�6^   �� 5X      -   5        $��Wybierz��5X   !   $��80��63   6>   5Y      �� 5\      -   5         ��Name$ SelectedPathTextBlock��5\       $��Nie wybrano katalogu��5]      $��0,10,0,0q�5^      $��Italic$�5_      
�������$��CharacterEllipsis=�5a       � ToolTip 5b      �� *Text+$ SelectedPathTextBlock��5c      �� 5f      -   5        $��ZAPISZ��5f      $	��100��6.   $��0,20,0,0q�6:   $��Center=�6L   6i   5h      5i      �� 5l   
   $��1��6   $��10q�6"   " �� VerticalScrollBarVisibility$
 Auto=�6.   $ �� HorizontalScrollBarVisibility$
 Auto=�6Q    �� TextWrapping$
 Wrap=�6v   $��Consolas)�6�   $��12(�6�    �� 
IsReadOnly$
 True��6�   ��6�   �� *Logs+ �� UpdateSourceTrigger$ PropertyChanged=�5m      5n      