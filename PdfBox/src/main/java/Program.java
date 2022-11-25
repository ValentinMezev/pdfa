public class Program {
    public static void main(String[] args) {
        System.out.println("generating");

        try {
            Generator generator = new Generator();
            generator.generateDocument("./input.pdf", "./output-file.pdf");
            System.out.println("done");
        } catch (Exception ex) {
            System.out.println(ex.getMessage());
        }
    }
}