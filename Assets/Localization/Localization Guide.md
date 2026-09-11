# Variables
Written like <span style="color: #55ff55;">#1#</span>
- Variables act as a way to insert values during runtime, such as numbers or types.
- Variables must have incrementing values, ergo <span style="color: #55ff55;">#1#</span>, <span style="color: #55ff55;">#2#</span>, <span style="color: #55ff55;">#3#</span>, etc. And will be replaced in the corresponding order.
### Examples
- <span style="color: #55ff55;">#1#</span> / <span style="color: #55ff55;">#2#</span> EXP
	- <span style="color: #ff00ff;">1 is 25, 2 is 100</span> 25 / 100 EXP
# Formatters
Written like <span style="color: #D8F02F;">[R=2]</span><span style="color: #555555;">#1#</span>
- Written before Variables to adjust their output or parameters.
### List of formatters
- <span style="color: #D8F02F;">[R=X]</span> Rounds a number to X decimals
- <span style="color: #D8F02F;">[P=X]</span> Ensures a number has at least X digits in resulting string
### Examples
- <span style="color: #D8F02F;">[R=1]</span><span style="color: #55ff55;">#1#</span> Life regeneration
	- <span style="color: #ff00ff;">X is 1.2503</span> 1.3 Life Regeneration
- <span style="color: #D8F02F;">[P=5]</span><span style="color: #55ff55;">#1#</span> Power
	- <span style="color: #ff00ff;">X is 25</span> 00025 Power
# Conditionals
Written like <span style="color: #7777ff;">{Text1|Text2}</span>
- Conditionals allow for a way to adjust wording based on a condition during run time.
- Examples of this could be for multiples or when values increment or decrement.
### Examples
- <span style="color: #55ff55;">#1#</span>% <span style="color: #7777ff;">{Increased|Decreased}</span> Damage
	- <span style="color: #ff00ff;">Where X is greater than 0</span> 100% Increased Damage 
	- <span style="color: #ff00ff;">Where X is less than 0</span> 100% Decreased Damage
- Player<span style="color: #7777ff;">{|s}</span> Damage
	- <span style="color: #ff00ff;">Where X is 1</span> Player Damage
	- <span style="color: #ff00ff;">Where X is not 1</span> Players Damage