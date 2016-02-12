<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0"
    xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:wix="http://schemas.microsoft.com/wix/2006/wi"
    xmlns="http://schemas.microsoft.com/wix/2006/wi"
    exclude-result-prefixes="xsl wix">

  <xsl:output method="xml" indent="yes"/>

  <!-- Identity Transformation -->
  <xsl:template match="@* | node()">
    <xsl:copy>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

  <!-- Insert <?include?> to source file paths.wxi -->
  <xsl:template match="wix:Wix">
    <!-- Copy the element -->
    <xsl:copy>
      <!-- Add new node (or whatever else you wanna do) -->
      <xsl:text disable-output-escaping="yes">
        &lt;&#63;include ..\..\Includes\SourceFilePaths.wxi&#63;&gt;
        </xsl:text>
      <xsl:apply-templates select="@* | node()"/>
    </xsl:copy>
  </xsl:template>

</xsl:stylesheet>
