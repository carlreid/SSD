using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace SSD
{
    /// <summary>
    /// A class to generate terrain with no heights, random heights or heights obtained from a heightmap
    /// Provided by Keith Ditchburn for any purpose including use in your GSD assignment
    /// Updated 15/3/11 to provide a GetHeightAtPoint function (note: requires a square terrain)
    /// </summary>
    public class Terrain
    {
        VertexPositionNormalTexture[] m_vertices;
        int[] m_indices;        
        Texture2D m_texture;
        BasicEffect m_effect;
        int m_numTriangles;
        int m_numVertices;

        // For height at point calculations:
        Vector3 m_minBounds;
        Vector3 m_maxBounds;
        int m_numCellsX;
        int m_numCellsZ;
        Vector3[] m_triA;
        Vector3[] m_triB;

        public Terrain()
        {
            // Precreate two unit triangles, A and B for later use in height at point
            m_triA = new Vector3[3];
            m_triA[0] = new Vector3(0, 0, 0);
            m_triA[1] = new Vector3(1, 0, 1);
            m_triA[2] = new Vector3(1, 0, 0);

            m_triB = new Vector3[3];
            m_triB[0] = new Vector3(0, 0, 0);
            m_triB[1] = new Vector3(0, 0, 1);
            m_triB[2] = new Vector3(1, 0, 1);
        }

        /// <summary>
        /// Creates a flat terrain to the supplied world dimensions. Centres on 0,0,0.
        /// </summary>
        /// <param name="Content">content manager</param>
        /// <param name="device">graphics device</param>
        /// <param name="textureName">texture to map onto the terrain</param>        
        /// <param name="worldSizeX">number of world units the terrain covers in the x axis</param>
        /// <param name="worldSizeZ">number of world units the terrain covers in the z axis</param>
        /// <returns>false if an error occured</returns>
        public bool CreateFlatTerrain(ContentManager content, GraphicsDevice device, string textureName,
            int worldSizeX, int worldSizeZ)
        {
            CreateTriangles(content, textureName, worldSizeX, worldSizeZ, 4, 4, 0);
            CreateEffect(device);
            CreateNormals();

            return true;
        }

        /// <summary>
        /// Creates a bumpy terrain to the supplied world dimensions, number of cells and maximum height, Centres on 0,0,0.
        /// </summary>
        /// <param name="Content">content manager</param>
        /// <param name="device">graphics device</param>
        /// <param name="textureName">texture to map onto the terrain</param>        
        /// <param name="worldSizeX">number of world units the terrain covers in the x axis</param>
        /// <param name="worldSizeZ">number of world units the terrain covers in the z axis</param>
        /// <param name="numCellsX">number of cells to create in the x axis</param>
        /// <param name="numCellsZ">number of cells to create in the z axis</param>
        /// <param name="maxY">maximum y value of each vertex</param>
        /// <returns>false if an error occured</returns>
        public bool CreateRandomBumpyTerrain(ContentManager content, GraphicsDevice device, string textureName,
            int worldSizeX, int worldSizeZ, int numCellsX, int numCellsZ, int maxY)
        {
            CreateTriangles(content, textureName, worldSizeX, worldSizeZ, numCellsX, numCellsZ, maxY);
            CreateEffect(device);
            CreateNormals();
            return true;
        }

        /// <summary>
        /// Creates a terrain to the supplied world dimensions, number of cells and heights from a texture, Centres on 0,0,0.
        /// </summary>
        /// <param name="Content">content manager</param>
        /// <param name="device">graphics device</param>
        /// <param name="textureName">texture to map onto the terrain</param>        
        /// <param name="heightMapName">texture used to set heights</param>        
        /// <param name="worldSizeX">number of world units the terrain covers in the x axis</param>
        /// <param name="worldSizeZ">number of world units the terrain covers in the z axis</param>
        /// <param name="numCellsX">number of cells to create in the x axis</param>
        /// <param name="numCellsZ">number of cells to create in the z axis</param>
        /// <param name="useLighting">enables of disables lighting</param>
        /// <returns>false if an error occured</returns>
        public bool CreateTerrainFromHeightMap(ContentManager Content, GraphicsDevice device, string textureName,
            string heightMapName,
            int worldSizeX, int worldSizeZ, int numCellsX, int numCellsZ, bool useLighting)
        {
            Texture2D heightMap = Content.Load<Texture2D>(heightMapName);

            // Store colours from heightmap
            int totalSize = heightMap.Width * heightMap.Height;
            Color[] texColours = new Color[totalSize];
            heightMap.GetData<Color>(texColours);

            // Create terrain as normal
            CreateTriangles(Content, textureName, worldSizeX, worldSizeZ, numCellsX, numCellsZ, 0);
            //if (!Initialise(Content, textureName, vis, worldSizeX, worldSizeZ, numCellsX, numCellsZ))
            //  return false;

            // Now fill in heights from heightmap
            int numVerticesX = numCellsX + 1;
            int numVerticesZ = numCellsZ + 1;

            // The heightmap may be bigger resolution to the cell size so need to map:
            float xLookupRatio = (float)heightMap.Width / (float)numVerticesX;
            float zLookupRatio = (float)heightMap.Height / (float)numVerticesZ;

            int count = 0;
            float zLookup = 0.0f;
            for (int z = 0; z < numVerticesZ; z++)
            {
                float xLookup = 0.0f;
                int zLookUpInt = (int)zLookup;

                for (int x = 0; x < numVerticesX; x++)
                {
                    int xLookUpInt = (int)xLookup;

                    int texel = zLookUpInt * heightMap.Width + xLookUpInt;

                    // Red channel used but could be any
                    m_vertices[count++].Position.Y = texColours[texel].R;

                    xLookup += xLookupRatio;
                }
                zLookup += zLookupRatio;
            }

            // Effect
            CreateEffect(device);

            // Normalise
            CreateNormals();

            if (!useLighting)
            {
                // Lightmap provided
                m_effect.LightingEnabled = false;
            }

            return true;
        }

        /// <summary>
        /// Create a triangle grid made up of equal cells with two triangles per cell
        /// </summary>
        private void CreateTriangles(ContentManager Content, string textureName,
            int worldSizeX, int worldSizeZ, int numCellsX, int numCellsZ, int maxHeight)
        {
            m_texture = Content.Load<Texture2D>(textureName);

            int numVerticesX = numCellsX + 1;
            int numVerticesZ = numCellsZ + 1;
            int numTrianglesWide = numCellsX * 2;
            int numTrianglesHigh = numCellsZ;

            m_numVertices = numVerticesX * numVerticesZ;
            m_numTriangles = numTrianglesWide * numTrianglesHigh;

            m_indices = new int[m_numTriangles * 3];
            m_vertices = new VertexPositionNormalTexture[m_numVertices];

            int startZPosition = -worldSizeZ / 2;
            int startXPosition = -worldSizeX / 2;

            int cellXSize = worldSizeX / numCellsX;
            int cellZSize = worldSizeZ / numCellsZ;

            // Store some values for later use in obtaining the height
            m_minBounds = new Vector3(startXPosition, 0, startZPosition);
            m_maxBounds = new Vector3(startXPosition + worldSizeX, 0, startZPosition + worldSizeZ);
            m_numCellsX = numCellsX;
            m_numCellsZ = numCellsZ;

            Random rnd = new Random();

            // Fill in the vertices
            int count = 0;
            int worldZPosition = startZPosition;
            for (int z = 0; z < numVerticesZ; z++)
            {
                int worldXPosition = startXPosition;
                for (int x = 0; x < numVerticesX; x++)
                {
                    if (maxHeight == 0)
                        m_vertices[count].Position = new Vector3(worldXPosition, maxHeight, worldZPosition);
                    else
                        m_vertices[count].Position = new Vector3(worldXPosition, rnd.Next(maxHeight), worldZPosition);

                    m_vertices[count].Normal = Vector3.Up;
                    m_vertices[count].TextureCoordinate.X = (float)x / (numVerticesX - 1);
                    m_vertices[count].TextureCoordinate.Y = (float)z / (numVerticesZ - 1);

                    count++;

                    worldXPosition += cellXSize;
                }
                worldZPosition += cellZSize;
            }

            // Now the indices
            int index = 0;
            for (int cellZ = 0; cellZ < numCellsZ; cellZ++)
            {
                for (int cellX = 0; cellX < numCellsX; cellX++)
                {
                    // Each cell has two triangles
                    // First triangle uses vertices: 0,1,vertsWide
                    // Second uses: 1,vertsWide+1,vertsWide
                    int offset = cellZ * numVerticesX + cellX;

                    m_indices[index] = offset + 0;
                    m_indices[index + 1] = offset + 1;
                    m_indices[index + 2] = offset + numVerticesX;

                    index += 3;

                    m_indices[index] = offset + 1;
                    m_indices[index + 1] = offset + numVerticesX + 1;
                    m_indices[index + 2] = offset + numVerticesX;

                    index += 3;
                }
            }
        }

        /// <summary>
        /// Create and fill in an effect for the terrain
        /// </summary>
        private void CreateEffect(GraphicsDevice device)
        {
            // Effect
            m_effect = new BasicEffect(device);
            m_effect.TextureEnabled = true;
            m_effect.Texture = m_texture;
            m_effect.PreferPerPixelLighting = true;

            // TODO: lighting direction will depend on your demo
            m_effect.LightingEnabled = true;
            m_effect.DirectionalLight0.Enabled = true;
            m_effect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(0, -0.9f, 0.5f));
            m_effect.DirectionalLight0.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);

            m_effect.AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f);
        }

        /// <summary>
        /// Create terrain vertex guaraud normals
        /// </summary>
        private void CreateNormals()
        {
            // Normalise

            // 1. Calculate plane normals
            Vector3[] triNormals = new Vector3[m_numTriangles];
            for (int t = 0; t < m_numTriangles; t++)
            {
                Vector3 A = m_vertices[m_indices[t * 3]].Position;
                Vector3 B = m_vertices[m_indices[t * 3 + 1]].Position;
                Vector3 C = m_vertices[m_indices[t * 3 + 2]].Position;

                Vector3 Side1 = B - A;
                Vector3 Side2 = C - A;

                triNormals[t] = Vector3.Cross(Side2, Side1);
                triNormals[t].Normalize();
            }

            // 2. Calculate vertex normals by finding all triangles that use this vertex
            for (int i = 0; i < m_numVertices; i++)
            {
                Vector3 total = Vector3.Zero;
                int numNormalsAdded = 0;
                for (int j = 0; j < m_numTriangles * 3; j++)
                {
                    if (m_indices[j] == i)
                    {
                        total += triNormals[j / 3];
                        numNormalsAdded++;
                    }
                }

                m_vertices[i].Normal = total / (float)numNormalsAdded;
            }
        }
        /// <summary>
        /// Render the terrain
        /// </summary>
        public void Render(GraphicsDevice device, Matrix world, Matrix proj, Matrix view)
        {
            m_effect.World = world;
            m_effect.View = view;
            m_effect.Projection = proj;

            foreach (EffectPass pass in m_effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                //device.Indices.IndexElementSize = IndexElementSize.ThirtyTwoBits;
                device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, m_vertices,
                    0, m_vertices.Length, m_indices, 0, m_indices.Length / 3);
            }
        }

        // Using method from:
        // http://gpwiki.org/index.php/MathGem:Height_of_a_Point_in_a_Triangle
        private float GetHeightInTri(Vector3[] v, float x, float z)
        {
            // Normal
            Vector3 nv1 = v[1] - v[0];
            Vector3 nv2 = v[2] - v[0];

            Vector3 n = Vector3.Zero;
            Vector3.Cross(ref nv1, ref nv2, out n);
            n.Normalize();

            Trace.Assert(n.Y >= 0.0f);

            // The height is distance in x axis times the x normal plus the distance in the z axis times the z normal
            // the whole lot is then divided by the negative y normal to scale it down correctly (otherwise the y value would
            // not have been taken into account). The result is added to the vertex y value to give the actual height.
            // Note that this equation is derived from the fact that the dot product of the plane and the vector between two points
            // on it is 0. 0 = N dot (X-P) where X is a point on the plane and so is P (a vertex on the plane)
            // If you multiply out that equation and rearrange you get the equation below:
            float y = v[0].Y + ((n.Z * (z - v[0].Z) + n.X * (x - v[0].X)) / -n.Y);

            return y;

        }

        // Height at point
        public float GetHeightAtPoint(Vector3 point)
        {
            // Get point relative to bottom left of terrain
            Vector3 pointOffset = point - m_minBounds;

            // Check for out of range, if so just return height of 0
            if (pointOffset.X < 0.0f || pointOffset.Z < 0.0f)
                return 0.0f;
            if (point.X >= m_maxBounds.X || point.Z >= m_maxBounds.Z)
                return 0.0f;

            float cellWidth = (m_maxBounds.X - m_minBounds.X) / (float)(m_numCellsX);
            float cellHeight = (m_maxBounds.Z - m_minBounds.Z) / (float)(m_numCellsZ);

            // Find out which cell we are in
            int cellX = (int)(pointOffset.X / cellWidth);
            int cellZ = (int)(pointOffset.Z / cellHeight);

            int verticesPerRow = m_numCellsX + 1;

            int vertIndex0 = cellZ * verticesPerRow + cellX;
            int vertIndex1 = vertIndex0 + 1;
            int vertIndex2 = vertIndex0 + verticesPerRow;
            int vertIndex3 = vertIndex0 + verticesPerRow + 1;

            // Since the terrain cells are square we can work out which tri
            float dx = pointOffset.X - ((float)cellX * cellWidth);
            float dz = pointOffset.Z - ((float)cellZ * cellHeight);

            float height = 0.0f;
            // Get x,z as 0 to 1 values in a unit tri
            float px = dx / cellWidth;
            float pz = dz / cellHeight;

            Trace.Assert(px >= 0.0f && px <= 1.0f);
            Trace.Assert(pz >= 0.0f && pz <= 1.0f);

            /*
            Unit cell
                2 ********* 3
                  *       *
                  *       *
                  *       *
                0 ********* 1
            */

            // Triangles are set up as unit tris at start, just need to fill in vertex heights
            m_triA[0].Y = m_vertices[vertIndex0].Position.Y;
            m_triA[1].Y = m_vertices[vertIndex3].Position.Y;
            m_triA[2].Y = m_vertices[vertIndex1].Position.Y;

            m_triB[0].Y = m_vertices[vertIndex0].Position.Y;
            m_triB[1].Y = m_vertices[vertIndex2].Position.Y;
            m_triB[2].Y = m_vertices[vertIndex3].Position.Y;

            if (dx > dz)// must be in 0,1,3 triangle
                height = GetHeightInTri(m_triA, px, pz);
            else// must be in 0,3,2 triangle
                height = GetHeightInTri(m_triB, px, pz);

            return height;
        }
    }
}
