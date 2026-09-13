using System;
using System.Collections.Generic;

class Program
{
    static List<string> nombres = new List<string>();
    static List<decimal> precios = new List<decimal>();
    static List<int> stocks = new List<int>();
    static List<int> unidadesVendidas = new List<int>();

    static decimal totalCaja = 0;
    static int totalVentas = 0;

    static void Main()
    {
        int opcion;

        do
        {
            ImprimirEncabezado("SISTEMA GESTOR DE VENTAS E INVENTARIO");

            Console.WriteLine("1. Registrar nuevo producto");
            Console.WriteLine("2. Consultar inventario completo");
            Console.WriteLine("3. Registrar una venta");
            Console.WriteLine("4. Ver reporte de caja y estadísticas");
            Console.WriteLine("5. Salir");

            opcion = LeerEntero("Seleccione una opción: ", 1, 5);

            switch (opcion)
            {
                case 1:
                    RegistrarProducto();
                    break;

                case 2:
                    ConsultarInventario();
                    break;

                case 3:
                    RegistrarVenta();
                    break;

                case 4:
                    VerReporte();
                    break;

                case 5:
                    Console.WriteLine("\nGracias por utilizar el sistema. ¡Hasta luego!");
                    break;
            }

            if (opcion != 5)
            {
                Console.WriteLine("\nPresione ENTER para continuar...");
                Console.ReadLine();
            }

        } while (opcion != 5);
    }

    static void RegistrarProducto()
    {
        Console.WriteLine("\n--- REGISTRAR PRODUCTO ---");

        Console.Write("Nombre del producto: ");
        string nombre = Console.ReadLine() ?? "";

        while (string.IsNullOrWhiteSpace(nombre))
        {
            Console.Write("El nombre no puede estar vacío. Intente nuevamente: ");
            nombre = Console.ReadLine() ?? "";
        }

        // Evitar productos duplicados
        for (int i = 0; i < nombres.Count; i++)
        {
            if (nombres[i].Equals(nombre, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("\nEse producto ya está registrado.");
                return;
            }
        }

        decimal precio = LeerDecimal("Precio unitario: ", 0.01m);
        int stock = LeerEntero("Stock inicial: ", 0, int.MaxValue);

        nombres.Add(nombre);
        precios.Add(precio);
        stocks.Add(stock);
        unidadesVendidas.Add(0);

        Console.WriteLine("\nProducto registrado correctamente.");
    }

    static void ConsultarInventario()
    {
        Console.WriteLine("\n--- INVENTARIO ---");

        if (nombres.Count == 0)
        {
            Console.WriteLine("El inventario está vacío.");
            return;
        }

        for (int i = 0; i < nombres.Count; i++)
        {
            Console.WriteLine(
                $"{i + 1}. {nombres[i]} | Precio: {precios[i]:C} | Stock: {stocks[i]}"
            );

            if (stocks[i] < 5)
            {
                Console.WriteLine("   [ALERTA: BAJO STOCK]");
            }
        }
    }

    static void RegistrarVenta()
    {
        Console.WriteLine("\n--- REGISTRAR VENTA ---");

        if (nombres.Count == 0)
        {
            Console.WriteLine("No hay productos registrados.");
            return;
        }

        bool hayStock = false;

        for (int i = 0; i < stocks.Count; i++)
        {
            if (stocks[i] > 0)
            {
                hayStock = true;
                break;
            }
        }

        if (!hayStock)
        {
            Console.WriteLine("\nNo hay productos disponibles para vender.");
            return;
        }

        Console.WriteLine("Productos disponibles:");

        for (int i = 0; i < nombres.Count; i++)
        {
            if (stocks[i] > 0)
            {
                Console.WriteLine(
                    $"{i + 1}. {nombres[i]} - {precios[i]:C} - Stock: {stocks[i]}"
                );
            }
            else
            {
                Console.WriteLine(
                    $"{i + 1}. {nombres[i]} - AGOTADO"
                );
            }
        }

        int producto = LeerEntero(
            "Seleccione el número del producto: ",
            1,
            nombres.Count
        );

        int indice = producto - 1;

        if (stocks[indice] == 0)
        {
            Console.WriteLine("\nEse producto está agotado.");
            return;
        }

        int cantidad;

        while (true)
        {
            cantidad = LeerEntero(
                "Cantidad a vender: ",
                1,
                int.MaxValue
            );

            if (cantidad <= stocks[indice])
            {
                break;
            }

            Console.WriteLine(
                $"No hay suficiente stock. Solo quedan {stocks[indice]} unidades."
            );
        }

        Console.Write("¿Cliente frecuente? (S/N): ");
        string respuesta = (Console.ReadLine() ?? "").ToUpper();

        bool tieneDescuento = respuesta == "S";

        decimal subtotal = precios[indice] * cantidad;

        decimal total = CalcularFactura(
            precios[indice],
            cantidad,
            tieneDescuento,
            out decimal iva,
            out decimal descuento
        );

        stocks[indice] -= cantidad;
        unidadesVendidas[indice] += cantidad;

        totalCaja += total;
        totalVentas++;

        Console.WriteLine("\n========== TICKET ==========");
        Console.WriteLine($"Producto: {nombres[indice]}");
        Console.WriteLine($"Cantidad: {cantidad}");
        Console.WriteLine($"Precio unitario: {precios[indice]:C}");
        Console.WriteLine($"Subtotal: {subtotal:C}");
        Console.WriteLine($"Descuento: {descuento:C}");
        Console.WriteLine($"IVA (19%): {iva:C}");
        Console.WriteLine($"TOTAL: {total:C}");
        Console.WriteLine("============================");
    }

    static void VerReporte()
    {
        Console.WriteLine("\n--- REPORTE DE CAJA Y ESTADÍSTICAS ---");

        Console.WriteLine($"Total de ventas: {totalVentas}");
        Console.WriteLine($"Dinero acumulado: {totalCaja:C}");

        if (totalVentas > 0)
        {
            decimal promedio = totalCaja / totalVentas;
            Console.WriteLine($"Promedio por venta: {promedio:C}");
        }
        else
        {
            Console.WriteLine("Promedio por venta: $0");
        }

        if (nombres.Count > 0)
        {
            int mayor = 0;
            int indiceMayor = 0;

            for (int i = 0; i < unidadesVendidas.Count; i++)
            {
                if (unidadesVendidas[i] > mayor)
                {
                    mayor = unidadesVendidas[i];
                    indiceMayor = i;
                }
            }

            if (mayor > 0)
            {
                Console.WriteLine(
                    $"Producto con más unidades vendidas: {nombres[indiceMayor]} - {mayor} unidades"
                );
            }
            else
            {
                Console.WriteLine(
                    "Producto con más unidades vendidas: todavía no hay ventas."
                );
            }
        }
    }

    static int LeerEntero(string mensaje, int min, int max)
    {
        while (true)
        {
            Console.Write(mensaje);

            if (int.TryParse(Console.ReadLine(), out int valor))
            {
                if (valor >= min && valor <= max)
                {
                    return valor;
                }
            }

            Console.WriteLine(
                $"Valor inválido. Debe estar entre {min} y {max}."
            );
        }
    }

    static decimal LeerDecimal(string mensaje, decimal min)
    {
        while (true)
        {
            Console.Write(mensaje);

            if (decimal.TryParse(Console.ReadLine(), out decimal valor))
            {
                if (valor >= min)
                {
                    return valor;
                }
            }

            Console.WriteLine(
                $"Valor inválido. Debe ser mayor o igual a {min}."
            );
        }
    }

    static decimal CalcularFactura(
        decimal precio,
        int cantidad,
        bool tieneDescuento,
        out decimal montoIva,
        out decimal montoDescuento)
    {
        decimal subtotal = precio * cantidad;

        if (tieneDescuento)
        {
            montoDescuento = subtotal * 0.10m;
        }
        else
        {
            montoDescuento = 0;
        }

        decimal baseIva = subtotal - montoDescuento;

        montoIva = baseIva * 0.19m;

        decimal total = baseIva + montoIva;

        return total;
    }

    static void ImprimirEncabezado(string titulo)
    {
        Console.Clear();

        Console.WriteLine("==============================================");
        Console.WriteLine($"        {titulo}");
        Console.WriteLine("==============================================");
        Console.WriteLine();
    }
}