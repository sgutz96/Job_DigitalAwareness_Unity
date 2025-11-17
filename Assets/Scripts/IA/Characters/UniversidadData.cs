using System;

[System.Serializable]
public class UniversidadData
{
    public Universidad universidad;
}

[System.Serializable]
public class Universidad
{
    public string nombre;
    public string sigla;
    public string sede;
    public Programa[] programas;
}

[System.Serializable]
public class Programa
{
    public string nombre;
    public string facultad;
    public string codigo_SNIES;
    public string titulo_otorgado;
    public string duracion;
    public string modalidad;
    public string jornada;

    public RegistroCalificado registro_calificado;
    public AcreditacionAltaCalidad acreditacion_alta_calidad;
    public Costo costo;
    public PlanEstudios plan_de_estudios;

    public string descripcion;

    public string[] objetos_de_estudio;
    public string[] perfil_egresado;
    public string[] campos_de_accion;
}

[System.Serializable]
public class RegistroCalificado
{
    public string resolucion;
    public string fecha;
    public string vigencia;
}

[System.Serializable]
public class AcreditacionAltaCalidad
{
    public string resolucion;
    public string fecha;
    public string vigencia;
}

[System.Serializable]
public class Costo
{
    public int valor_semestre;
    public string moneda;
    public string nota;
}

[System.Serializable]
public class PlanEstudios
{
    public string version;
    public string enlace;
}
