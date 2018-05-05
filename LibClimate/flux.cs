using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibClimate
{
    public class flux
    {
        public const int WIND = 0;
        public const int SURFACE_AND_MARINE_CURRENT = 1;

        public static bool transferEnergy(long i_source, long j_source, long i_target, long j_target, int directionFlow, TClima clima, double e_flux, double[,] energy_grid, int i, int j, double[,] copy_energy_grid, short[,] direction, int typeFlux, TWorld w, bool transfEnergy)
        {
            double
              energy_transferred,
              finalEnergy,
              T_radiated,
              contact_height,
              elevation,
              lat,
              factor;

            bool c = false;

            direction[i, j] = (short)directionFlow;
            // used for upper atmospheric layers
            if (!transfEnergy)
            {
                return true;
            }

            if (copy_energy_grid[i_source, j_source] < 0)
                throw new Exception("Energy negative at i:" + i + " and j:" + j);

            if (typeFlux == WIND)
            {
                // this is an additional factor to avoid melting of points
                factor = w.length_of_degree[j_source] / w.length_of_degree[Conversion.LatToY(0)];
                if (factor > 1) factor = 1;
                T_radiated = clima.T_atmosphere[0, i, j];
                energy_transferred = Math.Abs(energy_grid[i_source, j_source] - energy_grid[i_target, j_target])
                                      * e_flux * TSimConst.pct_wind_transfer * factor;

                {
                    // using Stefan Boltzmann the atmosphere is too static
                    elevation = w.elevation[i, j];
                    if (elevation < 0) elevation = 0;
                    contact_height = TSimConst.max_atmosphere_height - elevation;
                    energy_transferred = TPhysConst.stefan_boltzmann * (double)Math.Pow((double)T_radiated, 4)
                                  * averages.Avg(w.length_of_degree[j], w.length_of_degree[j_target])
                                  * contact_height * TSimConst.hour_step * e_flux *
                                  TSimConst.pct_wind_transfer;
                }
                if (energy_transferred < 0) throw new Exception("Energy transferred between squares is negative");
            }
            else
            {
                T_radiated = clima.T_ocean_terr[i, j];
                contact_height = averages.Avg(Math.Abs(w.elevation[i_target, j_target]), Math.Abs(w.elevation[i, j]));

                energy_transferred = TPhysConst.stefan_boltzmann * (double)Math.Pow((double)T_radiated, 4)
                                * averages.Avg(w.length_of_degree[j], w.length_of_degree[j_target]) * contact_height * TSimConst.hour_step * e_flux;
                if (energy_transferred < 0) throw new Exception("Energy transferred between squares is negative");
            }


            finalEnergy = energy_grid[i_source, j_source] - energy_transferred;
            if (finalEnergy < 0) return false;

            energy_grid[i_source, j_source] = finalEnergy;
            energy_grid[i_target, j_target] = energy_grid[i_target, j_target] + energy_transferred;

            return true;
        }

        public static void moveEnergy(TClima clima, double[,] energy_grid, double[,] copy_energy_grid, double[,] temp_grid, short[,] direction, int typeFlux, TWorld w, bool transfEnergy)
        {
            int i, j;
            long check_north,
            check_south,
            check_west,
            check_east;

            double T_north,
            T_south,
            T_west,
            T_east,
            T_north_west,
            T_north_east,
            T_south_west,
            T_south_east,
            T_own,
            T_lowestCardinal,
            T_lowestDiagonal,
            T_lowest;

            double e_flux;

            double lat;


            // we need a local copy of the energy grid
            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                    copy_energy_grid[i, j] = energy_grid[i, j];


            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                {

                    // initialize with the correct flux factor
                    if (typeFlux == WIND)
                        e_flux = (1 / TSimConst.exchange_flux_atm);
                    else
                        if (typeFlux == SURFACE_AND_MARINE_CURRENT)
                        {
                            if (w.isOcean[i, j] && (!clima.isIce[i, j]))
                                e_flux = (1 / TSimConst.exchange_flux_ocean);
                            else
                                e_flux = (1 / TSimConst.exchange_flux_terrain);
                        }
                        else
                            throw new Exception("typeFlux is unknown");

                    check_north = j - 1;
                    check_south = j + 1;
                    check_west = i - 1;
                    check_east = i + 1;

                    // we live on a sphere
                    if (check_north < 0) check_north = 179;
                    if (check_south > 179) check_south = 0;
                    if (check_west < 0) check_west = 359;
                    if (check_east > 359) check_east = 0;

                    T_north = temp_grid[i, check_north];
                    T_south = temp_grid[i, check_south];
                    T_west = temp_grid[check_west, j];
                    T_east = temp_grid[check_east, j];
                    T_north_west = temp_grid[check_west, check_north];
                    T_north_east = temp_grid[check_east, check_north];
                    T_south_west = temp_grid[check_west, check_south];
                    T_south_east = temp_grid[check_east, check_south];
                    T_own = temp_grid[i, j];


                    T_lowestCardinal = Math.Min(Math.Min(T_north, T_south), Math.Min(T_west, T_east));
                    T_lowestDiagonal = Math.Min(Math.Min(T_north_east, T_south_west), Math.Min(T_north_west, T_south_east));
                    T_lowest = Math.Min(T_lowestCardinal, T_lowestDiagonal);

                    if (T_lowest > T_own) continue;

                    // we move the energy into the place with lower temperature
                    // energy transfer between ocean and terrain is possible
                    // NOTE: the direction of winds and currents is dep}ing on invert_flow flag

                    if (T_own == T_lowest)
                    {
                        direction[i, j] = Constants.NONE;
                    }
                    else if (T_north == T_lowest)
                    {
                        transferEnergy(i, j, i, check_north, Constants.SOUTH * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                    else if (T_south == T_lowest)
                    {
                        transferEnergy(i, j, i, check_south, Constants.NORTH * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                    else if (T_west == T_lowest)
                    {
                        transferEnergy(i, j, check_west, j, Constants.EAST * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                    else if (T_east == T_lowest)
                    {
                        transferEnergy(i, j, check_east, j, Constants.WEST * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                    else if (T_south_west == T_lowest)
                    {
                        transferEnergy(i, j, check_west, check_south, Constants.NORTH_EAST * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                    else if (T_south_east == T_lowest)
                    {
                        transferEnergy(i, j, check_east, check_south, Constants.NORTH_WEST * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                    else if (T_north_west == T_lowest)
                    {
                        transferEnergy(i, j, check_west, check_north, Constants.SOUTH_EAST * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                    else if (T_north_east == T_lowest)
                    {
                        transferEnergy(i, j, check_east, check_north, Constants.SOUTH_WEST * TSimConst.invert_flow, clima, e_flux, energy_grid, i, j, copy_energy_grid, direction, typeFlux, w, transfEnergy);
                    }
                }
        }

        public static void coriolisClockwise(long i, long j, short[,] source_flow_grid, short[,] target_flow_grid)
        {
            // clockwise turn
            switch (source_flow_grid[i, j])
            {
                case Constants.NORTH:
                    target_flow_grid[i, j] = Constants.NORTH_WEST;
                    break;
                case Constants.SOUTH:
                    target_flow_grid[i, j] = Constants.SOUTH_EAST; break;
                case Constants.EAST:
                    target_flow_grid[i, j] = Constants.NORTH_EAST;//EAST;
                    break;
                case Constants.WEST:
                    target_flow_grid[i, j] = Constants.SOUTH_WEST;//WEST;
                    break;
                case Constants.NORTH_EAST:
                    target_flow_grid[i, j] = Constants.NORTH;
                    break;
                case Constants.NORTH_WEST:
                    target_flow_grid[i, j] = Constants.WEST;
                    break;
                case Constants.SOUTH_EAST:
                    target_flow_grid[i, j] = Constants.EAST;
                    break;
                case Constants.SOUTH_WEST:
                    target_flow_grid[i, j] = Constants.SOUTH;
                    break;
            }
        }

        public static void coriolisCounterClockwise(long i, long j, short[,] source_flow_grid, short[,] target_flow_grid)
        {
            // counterclockwise turn
            switch (source_flow_grid[i, j])
            {
                case Constants.NORTH:
                    target_flow_grid[i, j] = Constants.NORTH_WEST;
                    break;
                case Constants.SOUTH:
                    target_flow_grid[i, j] = Constants.SOUTH_EAST;
                    break;
                case Constants.EAST:
                    target_flow_grid[i, j] = Constants.NORTH_EAST;//EAST;
                    break;
                case Constants.WEST:
                    target_flow_grid[i, j] = Constants.SOUTH_WEST;//WEST;
                    break;
                case Constants.NORTH_EAST:
                    target_flow_grid[i, j] = Constants.NORTH;
                    break;
                case Constants.NORTH_WEST:
                    target_flow_grid[i, j] = Constants.WEST;
                    break;
                case Constants.SOUTH_EAST:
                    target_flow_grid[i, j] = Constants.EAST;
                    break;
                case Constants.SOUTH_WEST:
                    target_flow_grid[i, j] = Constants.SOUTH;
                    break;
            }
        }


        public static void applyCoriolis(short[,] source_flow_grid, short[,] target_flow_grid, bool apply)
        {
            long i, j;


            if (apply)
            {
                // north half sphere
                for (j = 0; j < 90; j++)
                    for (i = 0; i < 360; i++)
                        if (TMdlConst.inverse_rotation == 1)
                            coriolisClockwise(i, j, source_flow_grid, target_flow_grid); // normal rotation
                        else
                            coriolisCounterClockwise(i, j, source_flow_grid, target_flow_grid);

                // south half sphere
                for (j = 90; j < 180; j++)
                    for (i = 0; i < 360; i++)
                        if (TMdlConst.inverse_rotation == 1)
                            coriolisCounterClockwise(i, j, source_flow_grid, target_flow_grid); // normal rotation
                        else
                            coriolisClockwise(i, j, source_flow_grid, target_flow_grid);

            }
            else
            {
                // only copying if there is no coriolis
                for (j = 0; j < 180; j++)
                    for (i = 0; i < 360; i++)
                        target_flow_grid[i, j] = source_flow_grid[i, j];
            }
        }

        public static bool directionClose(short direction1, short direction2)
        {
            bool Result = false;
            if (direction1 == direction2)
            {
                return true;
            }

            switch (direction1)
            {
                case Constants.NORTH:
                    Result = (direction2 == Constants.NORTH_WEST) || (direction2 == Constants.NORTH_EAST);
                    break;
                case Constants.SOUTH:
                    Result = (direction2 == Constants.SOUTH_WEST) || (direction2 == Constants.SOUTH_EAST);
                    break;
                case Constants.EAST:
                    Result = (direction2 == Constants.NORTH_EAST) || (direction2 == Constants.SOUTH_EAST);
                    break;
                case Constants.WEST:
                    Result = (direction2 == Constants.NORTH_WEST) || (direction2 == Constants.SOUTH_WEST);
                    break;
                case Constants.NORTH_EAST:
                    Result = (direction2 == Constants.NORTH) || (direction2 == Constants.EAST);
                    break;
                case Constants.NORTH_WEST:
                    Result = (direction2 == Constants.NORTH) || (direction2 == Constants.WEST);
                    break;
                case Constants.SOUTH_EAST:
                    Result = (direction2 == Constants.SOUTH) || (direction2 == Constants.EAST);
                    break;
                case Constants.SOUTH_WEST:
                    Result = (direction2 == Constants.SOUTH) || (direction2 == Constants.WEST);
                    break;
            }

            return Result;
        }

        public static void changeDir(long i, long j, short proposedDir, short[,] flow_grid)
        {
            if (directionClose(flow_grid[i, j], proposedDir))
                flow_grid[i, j] = proposedDir;
        }

        public static void followSurface(TWorld w, short[,] flow_grid)
        {
            long
             i, j, m_north, m_south, m_west, m_east;
            double E_north,
            E_south,
            E_west,
            E_east,
            E_north_west,
            E_north_east,
            E_south_west,
            E_south_east,
            E_own,
            E_lowestCardinal,
            E_lowestDiagonal,
            E_lowest;

            for (j = 0; j < 180; j++)
                for (i = 0; i < 360; i++)
                {
                    if (w.isOcean[i, j]) continue;

                    m_north = j - 1;
                    m_south = j + 1;
                    m_west = i - 1;
                    m_east = i + 1;

                    // we live on a sphere
                    if (m_north < 0) m_north = 179;
                    if (m_south > 179) m_south = 0;
                    if (m_west < 0) m_west = 359;
                    if (m_east > 359) m_east = 0;

                    // check the surface gradient
                    E_north = w.elevation[i, m_north];
                    E_south = w.elevation[i, m_south];
                    E_west = w.elevation[m_west, j];
                    E_east = w.elevation[m_east, j];
                    E_north_west = w.elevation[m_west, m_north];
                    E_north_east = w.elevation[m_east, m_north];
                    E_south_west = w.elevation[m_west, m_south];
                    E_south_east = w.elevation[m_east, m_south];
                    E_own = w.elevation[i, j];

                    E_lowestCardinal = Math.Min(Math.Min(E_north, E_south), Math.Min(E_west, E_east));
                    E_lowestDiagonal = Math.Min(Math.Min(E_north_east, E_south_west), Math.Min(E_north_west, E_south_east));
                    E_lowest = Math.Min(E_lowestCardinal, E_lowestDiagonal);


                    if (E_own == E_lowest) continue;
                    else if (E_north == E_lowest)
                    {
                        changeDir(i, j, Constants.NORTH, flow_grid);
                    }
                    else if (E_south == E_lowest)
                    {
                        changeDir(i, j, Constants.SOUTH, flow_grid);
                    }
                    else if (E_west == E_lowest)
                    {
                        changeDir(i, j, Constants.WEST, flow_grid);
                    }
                    else if (E_east == E_lowest)
                    {
                        changeDir(i, j, Constants.EAST, flow_grid);
                    }
                    else if (E_south_west == E_lowest)
                    {
                        changeDir(i, j, Constants.SOUTH_WEST, flow_grid);
                    }
                    else if (E_south_east == E_lowest)
                    {
                        changeDir(i, j, Constants.SOUTH_EAST, flow_grid);
                    }
                    else if (E_north_west == E_lowest)
                    {
                        changeDir(i, j, Constants.NORTH_WEST, flow_grid);
                    }
                    else if (E_north_east == E_lowest)
                    {
                        changeDir(i, j, Constants.NORTH_EAST, flow_grid);
                    }
                }


        }
        public static void diffuseParticles(short direction, double particlesQty, double[,] particles, int i, int j, long m_north,
             long m_south, long m_west, long m_east)
        {
            if (direction == Constants.NONE)
                particles[i, j] = particles[i, j] + particlesQty;
            else if (direction == Constants.NORTH)
                particles[i, m_north] = particles[i, m_north] + particlesQty;
            else if (direction == Constants.SOUTH)
                particles[i, m_south] = particles[i, m_south] + particlesQty;
            else if (direction == Constants.WEST)
                particles[m_west, j] = particles[m_west, j] + particlesQty;
            else if (direction == Constants.EAST)
                particles[m_east, j] = particles[m_east, j] + particlesQty;
            else if (direction == Constants.NORTH_EAST)
                particles[m_east, m_north] = particles[m_east, m_north] + particlesQty;
            else if (direction == Constants.NORTH_WEST)
                particles[m_west, m_north] = particles[m_west, m_north] + particlesQty;
            else if (direction == Constants.SOUTH_EAST)
                particles[m_east, m_south] = particles[m_east, m_south] + particlesQty;
            else if (direction == Constants.SOUTH_WEST)
                particles[m_west, m_south] = particles[m_west, m_south] + particlesQty;
        }

        public static void moveParticlesInAtm(short[,] wind, double[,] particles, double[,] copyParticles)
        {
            short direction;
            int target_i, target_j;

            double speed,
            particlesQuantityCenter,
            particlesQuantityCardinal,
            particlesQuantityDiagonal;

            long m_north,
             m_south,
             m_west,
             m_east;

            // we need copy of the particles grid and we clear the actual one
            for (int j = 0; j < 180; j++)
                for (int i = 0; i < 360; i++)
                {
                    copyParticles[i, j] = particles[i, j];
                    particles[i, j] = 0;
                }


            for (int j = 0; j < 180; j++)
                for (int i = 0; i < 360; i++)
                {
                    m_north = j - 1;
                    m_south = j + 1;
                    m_west = i - 1;
                    m_east = i + 1;

                    // we live on a sphere
                    if (m_north < 0) m_north = 179;
                    if (m_south > 179) m_south = 0;
                    if (m_west < 0) m_west = 359;
                    if (m_east > 359) m_east = 0;

                    // TODO: adjust speed, and quantities for better diffusion effect
                    // if step=1h, degree_step=15
                    // speed = (TSimConst.degree_step/15);

                    // now we simply move the particles according to the wind
                    // diffusion percentages computed by
                    // D:\Projects\gpu_solar\src\dllbuilding\thedayaftertomorrow\docs\flux-computation.png
                    particlesQuantityCenter = copyParticles[i, j] * 0.1414710605;
                    particlesQuantityCardinal = copyParticles[i, j] * 0.1374730640;
                    particlesQuantityDiagonal = copyParticles[i, j] * 0.07715917088;

                    diffuseParticles(wind[i, j], particlesQuantityCenter, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[i, m_north], particlesQuantityCardinal, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[i, m_south], particlesQuantityCardinal, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[m_east, j], particlesQuantityCardinal, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[m_west, j], particlesQuantityCardinal, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[m_east, m_north], particlesQuantityDiagonal, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[m_east, m_south], particlesQuantityDiagonal, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[m_west, m_north], particlesQuantityDiagonal, particles, i, j, m_north, m_south, m_west, m_east);
                    diffuseParticles(wind[m_west, m_south], particlesQuantityDiagonal, particles, i, j, m_north, m_south, m_west, m_east);

                } // for
        }
    }
}
