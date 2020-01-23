public class Main {

	public static void main(String[] args) {

		int n = 120;
		int[] sieb = new int[n+1];

		//Sieb aufbauen
		for(int i = 2; i <= n; i++)
			sieb[i] = i;

		for(int i = 2; i <= n; i++) {
			if(sieb[i] == i) {
				//Zahl ist prim
				System.out.println(i);
				//Makiere alle Vielfachen von i
				for(int j = 2*i; j <= n; j=j+i)
					sieb[j] = i;
			}
		}
}
