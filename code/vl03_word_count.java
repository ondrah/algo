/*
 * Java version of VL03 EX0 using regex matching, thanks Marvin G.
 *
 * You probably need to rename this to main.java in order to compile and run.
 */

import java.util.regex.Pattern;
import java.util.regex.Matcher;
import java.util.*;

public class Main {
	public static void main(String[] args) {
		ArrayList<String> doublequoted = new ArrayList<String>();
		ArrayList<String> unqoted = new ArrayList<String>();

		String Text = "apple orange banana \"honey pie\" sun \"high noon\"";
		Pattern p = Pattern.compile("[^\\s\"']+|\"([^\"]*) \"|'([^']*)'");
		Matcher m = p.matcher(Text);
		while(m.find()) {
			if(m.group(1) != null) {
				//Woerter mit Anfuehrungszeichen
				doublequoted.add(m.group(1));
			} else {
				//Woerter ohne Anfuehrungszeichen
				unqoted.add(m.group());
			}
		}

		System.out.println("all expressions: " + (doublequoted.size() + unqoted.size()));
		System.out.println("compound expression: " + doublequoted.size());
	}
}
